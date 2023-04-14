package com.goods_account_service;

import java.net.CookieManager;
import java.net.HttpCookie;
import java.net.URI;

import java.lang.NullPointerException;
import java.util.List;

// This servise works with access data.
public class AccessService{
    
    /**
     * Singleton constructor.
     */
    protected AccessService(){
        _cookieManager = new java.net.CookieManager();
    }

    public static AccessService getInstance(){
        return _instance;
    }

    /**
     * Substraction JWT from response body and save into local storage.
     * @param response
     * @return Modified response.
     */
    public String sustractAndSaveJWT(String response){
        return "";
    }

    /**
     * @return JWT from storage or empty string.
     */
    public String loadJWT(){
        return "";
    }

    /**
     * Removing JWT fron storage.
     */
    public void removeJWT(){
    }

    /**
     * Saving httpOnly cookies
     * @param uri - resource uri.
     * @param allCookies - collection with cookies.
     */
    public void saveCookie(URI uri, Iterable<String> allCookies) throws NullPointerException
    {
        HttpCookie oldCookie = loadCookie(uri);
        if (oldCookie != null){
            _cookieManager.getCookieStore().remove(uri, loadCookie(uri));
        }
        
        for (String cookies : allCookies){
            for (HttpCookie cookie : (Iterable<HttpCookie>)HttpCookie.parse(cookies)){
                if (cookie.isHttpOnly()){
                    _cookieManager.getCookieStore().add(uri, cookie);
                    break;
                }
            }
        }
    }

    /**
     * Loading secure cookie.
     * @param uri - resource uri.
     * @return Secure cookie.
     */
    public HttpCookie loadCookie(URI uri) throws NullPointerException
    {
        Iterable<HttpCookie> cookies = (Iterable<HttpCookie>)_cookieManager.getCookieStore().get(uri);
        for (HttpCookie cookie : cookies){
            if (cookie.isHttpOnly()){
                return cookie;
            }
        }

        return null;
    }

    // Instance of this class.
    private final static AccessService _instance = new AccessService();
    
    // Cookie manager. 
    private static CookieManager _cookieManager;
}
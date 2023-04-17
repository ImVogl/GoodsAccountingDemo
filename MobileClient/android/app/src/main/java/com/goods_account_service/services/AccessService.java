package com.goods_account_service.services;

import com.goods_account_service.services.models.TokenResponse;

import java.net.CookieManager;
import java.net.HttpCookie;
import java.net.URI;

import java.lang.NullPointerException;

/**
 * This service works with access data.
 */
public final class AccessService
{
    /**
     * Instance of singleton.
     */
    private static AccessService _instance;

    /**
     * Cookie manager.
     */
    private CookieManager _cookieManager;

    /**
     * Current token.
     */
    private String _token;

    /**
     * Constructor.
     */
    private AccessService(){
        _cookieManager = new java.net.CookieManager();
    }

    public static AccessService getInstance(){
        if (_instance == null){
            _instance = new AccessService();
        }

        return _instance;
    }

    /**
     * Save JWT from response body into local storage.
     * @param response response with token.
     * @return Modified response.
     */
    public void saveJWT(TokenResponse response){
        _token = response.Token;
    }

    /**
     * @return JWT from storage or empty string.
     */
    public String loadJWT(){
        return _token;
    }

    /**
     * Removing JWT from storage.
     */
    public void removeJWT(){
        _token = "";
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

    /**
     * Converting HttpCookie object to String object.
     * @param cookie - cookie for conversion.
     * @return Cookie as string.
     */
    public String convertCookie(HttpCookie cookie)
    {
        String stringCookie = cookie.toString();
        stringCookie += "; path=" + cookie.getPath();
        if (cookie.getSecure()){
            stringCookie += "; secure";
        }

        if (cookie.isHttpOnly()){
            stringCookie += "; httponly";
        }

        return stringCookie;
    }
}

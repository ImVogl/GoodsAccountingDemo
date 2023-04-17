package com.goods_account_service;

import android.util.Log;
import java.util.logging.Logger;

import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactContextBaseJavaModule;
import com.facebook.react.bridge.ReactMethod;

import com.goods_account_service.services.ApiClient;
import com.goods_account_service.services.ClientResponse;

import java.io.StringWriter;
import java.io.PrintWriter;

import java.lang.IllegalStateException;

public class ClientModule extends ReactContextBaseJavaModule 
{
    // Empty json response body.
    private static final String EmptyJson = "{}";

    // Application context.
    private static ReactApplicationContext _reactContext;

    // This flag is indicating that this module was initialized.
    private static boolean _initialized;

    // Logger name.
    private static String _loggerName;

    // Api client.
    private ApiClient _apiClient;

    /**
     * @param context - application context.
     */
    ClientModule (ReactApplicationContext context) {
        super(context);
        _reactContext = context;
        _loggerName = this.getClass().getCanonicalName();
        _initialized = false;
    }

    /**
     * @return Name of this module.
     */
    @Override
    public String getName() {
        return "ApiClientModule";
    }

    /**
     * Initialization of module.
     * @param baseUrl - base api server url.
     * @param certificatePath - full path to certificate.
     * @param secretKeyPath - full path to secret key.
     */
    @ReactMethod
    public void init(String baseUrl, String certificatePath, String secretKeyPath)
    {
        _apiClient = new ApiClient(baseUrl, certificatePath, secretKeyPath);
        _initialized = true;

        Log.d(_loggerName, "Module was initialized");
    }

    /**
     * Authentification any user.
     * @param login - user's login
     * @param password - user's password
     * @param promise is a promise with response body in JSON.
     */
    @ReactMethod
    public void signin(String login, String password, Promise promise)
    {
        if (!checkInitializing(promise)){
            return;
        }
        
        ClientResponse response = _apiClient.signin(login, password);
        if (response.get_exception() == null){
            promise.resolve(response.get_response());
        } else{
            promise.reject(response.get_response(), response.get_exception());
        }
    }

    /**
     * Logout user.
     * @param id - user identificator.
     */
    public void signout(int id, Promise promise)
    {
        if (!checkInitializing(promise)){
            return;
        }
        
        ClientResponse response = _apiClient.signout(id);
        if (response.get_exception() == null){
            promise.resolve(response.get_response());
        } else{
            promise.reject(response.get_response(), response.get_exception());
        }
    }
    
    /**
     * Selling item.
     * @param id - user identificator.
     * @param item - item identificator.
     */
    public void sell(int id, String item, Promise promise)
    {
        if (!checkInitializing(promise)){
            return;
        }

        ClientResponse response = _apiClient.sell(id, item);
        if (response.get_exception() == null){
            promise.resolve(response.get_response());
        } else{
            promise.reject(response.get_response(), response.get_exception());
        }
    }
    
    /**
     * @param promise - result promise.
     * @return Value is indicating that object initialized.
     */
    private boolean checkInitializing(Promise promise)
    {
        if (_initialized){
            return true;
        }
        
        Log.d(_loggerName, "Module was not initialized!");
        promise.reject(new IllegalStateException("Module was not initialized!"));
        return false;
    }

     /**
      * Logging exception call stack.
     * @param exceptionName - exception name.
     * @param exception - Exception for converting.
     */
    private void logDebugException(String exceptionName, Exception exception)
    {
        StringWriter sw = new StringWriter();
        PrintWriter pw = new PrintWriter(sw);
        exception.printStackTrace(pw);
        Log.d(_loggerName, String.format("Name: %s;\n StackTrace: %s", exceptionName, sw.toString()));
    }
}
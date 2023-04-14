package com.goods_account_service;

import android.util.Log;
import java.util.logging.Logger;

import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactContextBaseJavaModule;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.uimanager.IllegalViewOperationException;

import com.goods_account_service.ClientResponse;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.FileInputStream;
import java.io.OutputStream;
import java.io.StringWriter;
import java.io.PrintWriter;
import java.io.IOException;
import java.io.UnsupportedEncodingException;

import java.lang.IllegalStateException;

import java.net.CookieManager;
import java.net.CookieStore;
import java.net.HttpCookie;
import java.net.URL;
import java.net.MalformedURLException;
import java.net.ProtocolException;
import java.net.URISyntaxException;

import javax.net.ssl.TrustManagerFactory;
import javax.net.ssl.HttpsURLConnection;
import javax.net.ssl.SSLContext;
import javax.net.ssl.TrustManager;
import javax.net.ssl.X509TrustManager;

import java.security.KeyStore;
import java.security.cert.CertificateFactory;
import java.security.cert.X509Certificate;
import java.security.NoSuchAlgorithmException;
import java.security.KeyManagementException;

import java.util.List;
import java.util.Map;
import java.util.HashMap;

public class ClientModule extends ReactContextBaseJavaModule {
    
    /**
     * @param context - application context.
     */
    ClientModule (ReactApplicationContext context) {
        super(context);
        _reactContext = context;
        _loggerName = this.getClass().getCanonicalName();
        _initialized = false;
        _dubug = false;
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
        _baseUrl = baseUrl;
        _certificatePath = certificatePath;
        _secretKeyPath = secretKeyPath;
        _cookieManager = new java.net.CookieManager();
        _initialized = true;

        Log.d(_loggerName, "Module was initialized");
    }

    /**
     * @param login - user's login
     * @param password - user's password
     * @param promise is a promise with response body in JSON.
     */
    @ReactMethod
    public void signin(String login, String password, Promise promise)
    {
        if (!_initialized){
            Log.d(_loggerName, "Module was not initialized!");
            promise.reject(new IllegalStateException("Module was not initialized!"));
            return;
        }

        String json = String.format("{\"login\": \"%s\", \"password\": \"%s\"}", login, password);
        ClientResponse response = post("/signin", json);
        if (response.get_exception() == null){
            promise.resolve(response.get_response());
        } else{
            promise.reject(response.get_response(), response.get_exception());
        }
    }
    
    // Empty json response body.
    private static final String EmptyJson = "{}";

    // Headers dictionary for item with cookie value.
    private static final String CookieKey = "Set-Cookie";

    // Application context.
    private static ReactApplicationContext _reactContext;

    // This flag is indicating that this module was initialized.
    private static boolean _initialized;

    // This flag is indicating that current mode is debug.
    private static boolean _dubug;

    // Full path to certificate.
    private static String _certificatePath;

    // Full path to secret key.
    private static String _secretKeyPath;

    // Base uri to API server.
    private static String _baseUrl;

    // Cookie manager. 
    private static CookieManager _cookieManager;

    // Logger name.
    private static String _loggerName;

    /**
     * Common post method.
     * @param path - subpath in url.
     * @param json - json body.
     * @return Data with resposponse.
     */
    private ClientResponse post(String path, String json){
        String errorMessage;
        try {
            SSLContext sslContext = SSLContext.getInstance("TLS");
            TrustManager[] manager = getTrustManager();
            sslContext.init(null, manager, new java.security.SecureRandom());
            HttpsURLConnection.setDefaultSSLSocketFactory(sslContext.getSocketFactory());
        } catch (NoSuchAlgorithmException exception) {
            logDebugException("NoSuchAlgorithmException", exception);
            return new ClientResponse(exception);
        } catch (KeyManagementException exception){
            logDebugException("KeyManagementException", exception);
            return new ClientResponse(exception);
        }

        try {
            URL url = new URL(_baseUrl.concat(path));
            HttpsURLConnection con = (HttpsURLConnection) url.openConnection();

            con.setRequestMethod("POST");
            con.setRequestProperty("Accept", "text/plain");
            con.setRequestProperty("Content-Type", "application/json");
            con.setDoOutput(true);

            try(OutputStream os = con.getOutputStream()) {
                byte[] input = json.getBytes();
                os.write(input, 0, input.length);
            }

            Map<String, List<String>> headers = con.getHeaderFields();
            if (!headers.keySet().contains(CookieKey)){
                try {
                    return new ClientResponse(EmptyJson);
                } catch (IllegalViewOperationException exception) {
                    logDebugException("IllegalViewOperationException", exception);
                    return new ClientResponse(exception);
                }
            }

            Iterable<String> allCookies = (Iterable<String>)headers.get(CookieKey);
            for (String cookie : allCookies){
                _cookieManager.getCookieStore().add(url.toURI(), HttpCookie.parse(cookie).get(0));
            }

            // Read the response
            BufferedReader in = new BufferedReader(new InputStreamReader(con.getInputStream()));
            String inputLine;
            StringBuffer local_response = new StringBuffer();
            while ((inputLine = in.readLine()) != null) {
                local_response.append(inputLine);
            }

            in.close();

            return new ClientResponse(local_response.toString());
        } catch (MalformedURLException exception) {
            logDebugException("MalformedURLException", exception);
            return new ClientResponse(exception);
        } catch (IllegalViewOperationException exception) {
            logDebugException("IllegalViewOperationException", exception);
            return new ClientResponse(exception);
        } catch (ProtocolException exception) {
            logDebugException("ProtocolException", exception);
            return new ClientResponse(exception);
        } catch (UnsupportedEncodingException exception){
            logDebugException("UnsupportedEncodingException", exception);
            return new ClientResponse(exception);
        } catch (URISyntaxException exception){
            logDebugException("URISyntaxException", exception);
            return new ClientResponse(exception);
        } catch (IOException exception) {
            logDebugException("IOException", exception);
            return new ClientResponse(exception);
        }
    }

    /**
     * @param crtPath - path to certificate file.
     * @param keyPath - path to key file.
     * @return Collection of <see ref="TrustManager" />.
     */
    private TrustManager[] getTrustManager() {
        try {
           // Load the .cer file
           FileInputStream cerInputStream = new FileInputStream(_certificatePath);
           CertificateFactory certificateFactory = CertificateFactory.getInstance("X.509");
           X509Certificate certificate = (X509Certificate) certificateFactory.generateCertificate(cerInputStream);
           
           // Load the .pem file
           FileInputStream pemInputStream = new FileInputStream(_secretKeyPath);
           byte[] pemBytes = new byte[pemInputStream.available()];
           pemInputStream.read(pemBytes);
           pemInputStream.close();
           
           // Create a KeyStore and add the certificate to it
           KeyStore keyStore = KeyStore.getInstance(KeyStore.getDefaultType());
           keyStore.load(null, null);
           keyStore.setCertificateEntry("alias", certificate);
           
           // Create a TrustManagerFactory and initialize it with the KeyStore
           TrustManagerFactory trustManagerFactory = TrustManagerFactory.getInstance(TrustManagerFactory.getDefaultAlgorithm());
           trustManagerFactory.init(keyStore);
           
           // Get the X509TrustManager from the TrustManagerFactory
           TrustManager[] trustManagers = trustManagerFactory.getTrustManagers();
           return trustManagers;
        } catch (Exception exception) {
            exception.printStackTrace();
        }

        return new TrustManager[0];
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
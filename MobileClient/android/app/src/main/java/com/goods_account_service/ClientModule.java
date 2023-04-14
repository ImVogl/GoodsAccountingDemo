package com.goods_account_service;

import android.util.Log;
import java.util.logging.Logger;

import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactContextBaseJavaModule;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.uimanager.IllegalViewOperationException;

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

        Log.d(this.getClass().getCanonicalName(), "Module was initialized");
    }

    /**
     * @param login - user's login
     * @param password - user's password
     * @param Promise with response body in JSON.
     */
    @ReactMethod
    public void signin(String login, String password, Promise response)
    {
        String errorMessage;
        if (!_initialized){
            response.resolve(new IllegalStateException("Module was not initialized!"));
            Log.d(this.getClass().getCanonicalName(), "Module was not initialized!");
            return;
        }

        try {
            SSLContext sslContext = SSLContext.getInstance("TLS");
            TrustManager[] manager = getTrustManager();
            sslContext.init(null, manager, new java.security.SecureRandom());
            HttpsURLConnection.setDefaultSSLSocketFactory(sslContext.getSocketFactory());
        } catch (NoSuchAlgorithmException exception) {
            errorMessage = convertException("NoSuchAlgorithmException", exception);
            response.resolve(errorMessage);
            Log.d(this.getClass().getCanonicalName(), errorMessage);
            return;
        } catch (KeyManagementException exception){
            errorMessage = convertException("KeyManagementException", exception);
            response.resolve(errorMessage);
            Log.d(this.getClass().getCanonicalName(), errorMessage);
            return;
        }

        try {
            URL url = new URL(_baseUrl.concat("/signin"));
            HttpsURLConnection con = (HttpsURLConnection) url.openConnection();

            con.setRequestMethod("POST");
            con.setRequestProperty("Accept", "text/plain");
            con.setRequestProperty("e_platform", "mobile");
            con.setRequestProperty("Content-Type", "application/json-patch+json");
            con.setRequestProperty("Authorization", null);
            con.setDoOutput(true);

            String json = String.format("{\"login\": \"%s\", \"password\": \"%s\"}", login, password);
            try(OutputStream os = con.getOutputStream()) {
                byte[] input = json.getBytes("utf-8");
                os.write(input, 0, input.length);			
            }

            Map<String, List<String>> headers = con.getHeaderFields();
            if (!headers.keySet().contains(CookieKey)){
                try {
                    response.resolve(EmptyJson);
                } catch (IllegalViewOperationException exception) {
                    errorMessage = convertException("IllegalViewOperationException", exception);
                    response.resolve(errorMessage);
                    Log.d(this.getClass().getCanonicalName(), errorMessage);
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
            response.resolve(local_response.toString());
        } catch (MalformedURLException exception) {
            errorMessage = convertException("MalformedURLException", exception);
            response.resolve(errorMessage);
            Log.d(this.getClass().getCanonicalName(), errorMessage);
        } catch (IllegalViewOperationException exception) {
            errorMessage = convertException("IllegalViewOperationException", exception);
            response.resolve(errorMessage);
            Log.d(this.getClass().getCanonicalName(), errorMessage);
        } catch (ProtocolException exception) {
            errorMessage = convertException("ProtocolException", exception);
            response.resolve(errorMessage);
            Log.d(this.getClass().getCanonicalName(), errorMessage);
        } catch (UnsupportedEncodingException exception){
            errorMessage = convertException("UnsupportedEncodingException", exception);
            response.resolve(errorMessage);
            Log.d(this.getClass().getCanonicalName(), errorMessage);
        } catch (URISyntaxException exception){
            errorMessage = convertException("URISyntaxException", exception);
            response.resolve(errorMessage);
            Log.d(this.getClass().getCanonicalName(), errorMessage);
        } catch (IOException exception) {
            errorMessage = convertException("IOException", exception);
            response.resolve(errorMessage);
            Log.d(this.getClass().getCanonicalName(), errorMessage);
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

    /**
     * @param crtPath - path to certificate file.
     * @param keyPath - path to key file.
     * @return Collection of <see ref="TrustManager" />.
     */
    private static TrustManager[] getTrustManager() {
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
      * Converting exception call stack to string.
     * @param exceptionName - exception name.
     * @param exception - Exception for converting.
     * @return Converted call stack.
     */
    private String convertException(String exceptionName, Exception exception)
     {
        StringWriter sw = new StringWriter();
        PrintWriter pw = new PrintWriter(sw);
        exception.printStackTrace(pw);
        return String.format("Name: %s;\n StackTrace: %s", exceptionName, sw.toString());
     }
}
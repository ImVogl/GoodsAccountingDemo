package Services;

import Services.Models.SoldItem;
import Services.Models.TokenResponse;
import Services.Models.UserInfo;
import com.google.gson.Gson;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.FileInputStream;
import java.io.OutputStream;
import java.io.StringWriter;
import java.io.PrintWriter;
import java.io.IOException;
import java.io.UnsupportedEncodingException;

import java.net.HttpCookie;
import java.net.URL;
import java.net.MalformedURLException;
import java.net.ProtocolException;
import java.net.URISyntaxException;

import javax.net.ssl.TrustManagerFactory;
import javax.net.ssl.HttpsURLConnection;
import javax.net.ssl.SSLContext;
import javax.net.ssl.TrustManager;

import java.security.KeyStore;
import java.security.cert.CertificateFactory;
import java.security.cert.X509Certificate;
import java.security.NoSuchAlgorithmException;
import java.security.KeyManagementException;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * Service class for api client.
 */
public class ApiClient
{
    /**
     * Headers dictionary for item with cookie value.
     */
    private static final String CookieKey = "Set-Cookie";

    /**
     * Full path to certificate.
     */
    private String _certificatePath;

    /**
     * Full path to secret key.
     */
    private String _secretKeyPath;

    /**
     * Base uri to API server.
     */
    private String _baseUrl;

    /**
     * Service for working with JWT and tokens.
     */
    private AccessService _accessService;

    /**
     * JSON data processor.
     */
    private Gson _gson;

    /**
     * Initializing new instance of ApiClient.
     * @param baseUrl - api service url.
     * @param certificatePath - path to certificate.
     * @param secretKeyPath - path to secret key.
     */
    public ApiClient(String baseUrl, String certificatePath, String secretKeyPath){
        _baseUrl = baseUrl;
        _certificatePath = certificatePath;
        _secretKeyPath = secretKeyPath;
        _accessService = AccessService.getInstance();
        _gson = new Gson();
    }

    /**
     * @param login - user's login.
     * @param password - user's password.
     * @return response body or error.
     */
    public String signin(String login, String password)
    {
        _accessService.removeJWT();
        String json = String.format("{\"login\": \"%s\", \"password\": \"%s\"}", login, password);
        ClientResponse response = post("/signin", json);
        if (response.get_exception() == null){
            UserInfo info = _gson.fromJson(response.get_response(), UserInfo.class);
            _accessService.saveJWT(info);
            info.Token = "";
            return _gson.toJson(info);
        } else{
            return response.get_response();
        }
    }

    /**
     * Signing out of user.
     * @param id - user identifier.
     */
    public boolean signout(int id)
    {
        String path = String.format("/signout/%s", id);
        ClientResponse response = post(path, null, true);
        if (response.get_exception() == null){
            _accessService.removeJWT();
            return true;
        } else{
            return false;
        }
    }
    /**
     * Selling item.
     * @param id - user identifier.
     * @param item - item identifier.
     */
    public boolean sell(int id, String item)
    {
        SoldItem soldItem = new SoldItem();
        soldItem.Id = id;
        soldItem.Sold = HashMap.newHashMap(0);
        soldItem.Sold.put(item, 1);

        String json = _gson.toJson(soldItem);
        ClientResponse response = post("/sold_goods", json);
        if (response.unautorized()){
            if (!refreshToken()){
                return false;
            };

            response = post("/sold_goods", json);
        }

        return response.get_exception() == null;
    }

    /**
     * @return Value is indicating that token updated
     */
    public boolean refreshToken(){
        ClientResponse response = post("/update_token", null, true);
        if (response.get_exception() == null){
            _accessService.saveJWT(_gson.fromJson(response.get_response(), TokenResponse.class));
            return true;
        };

        return false;
    }

    /**
     * Common post method.
     * @param path - sub path in url.
     * @param json - json body.
     * @return Data with response.
     */
    private ClientResponse post(String path, String json){
        return post(path, json, false);
    }

    /**
     * Common post method.
     * @param path - sub path in url.
     * @param json - json body.
     * @param withCredentials - value is indicating that request should have credentials cookie.
     * @return Data with response.
     */
    private ClientResponse post(String path, String json, boolean withCredentials){
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
            HttpsURLConnection connection = (HttpsURLConnection) url.openConnection();

            String jwt = _accessService.loadJWT();
            connection.setRequestMethod("POST");
            connection.setRequestProperty("Accept", "text/plain");
            if (!jwt.isEmpty()){
                connection.setRequestProperty("Authorization", "Bearer " + jwt);
            }

            HttpCookie currentCookie = _accessService.loadCookie(url.toURI());
            if (withCredentials & currentCookie != null){
                connection.setRequestProperty("Cookie", _accessService.convertCookie(currentCookie));
            }


            if (json != null){
                connection.setRequestProperty("Content-Type", "application/json");
                connection.setDoOutput(true);
                try(OutputStream os = connection.getOutputStream()) {
                    byte[] input = json.getBytes();
                    os.write(input, 0, input.length);
                }
            }

            int statusCode =  connection.getResponseCode();
            if (statusCode != 200){
                return new ClientResponse(statusCode);
            }

            Map<String, List<String>> headers = connection.getHeaderFields();
            if (headers.keySet().contains(CookieKey)){
                _accessService.saveCookie(url.toURI(), (Iterable<String>)headers.get(CookieKey));
            }

            // Read the response
            BufferedReader in = new BufferedReader(new InputStreamReader(connection.getInputStream()));
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
        } catch (ProtocolException exception) {
            logDebugException("ProtocolException", exception);
            return new ClientResponse(exception);
        } catch (UnsupportedEncodingException exception){
            logDebugException("UnsupportedEncodingException", exception);
            return new ClientResponse(exception);
        } catch (URISyntaxException exception){
            logDebugException("URISyntaxException", exception);
            return new ClientResponse(exception);
        }catch (NullPointerException exception) {
            logDebugException("NullPointerException", exception);
            return new ClientResponse(exception);
        } catch (IOException exception) {
            logDebugException("IOException", exception);
            return new ClientResponse(exception);
        }
    }

    /**
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
     * Converting exception call stack to string.
     * @param exceptionName - exception name.
     * @param exception - Exception for converting.
     * @return Converted call stack.
     */
    private static String convertException(String exceptionName, Exception exception)
    {
        StringWriter sw = new StringWriter();
        PrintWriter pw = new PrintWriter(sw);
        exception.printStackTrace(pw);
        return String.format("Name: %s;\n StackTrace: %s", exceptionName, sw.toString());
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
    }
}

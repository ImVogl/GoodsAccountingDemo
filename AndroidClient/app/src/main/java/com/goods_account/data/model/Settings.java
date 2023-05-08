package com.goods_account.data.model;

import java.io.FileNotFoundException;
import java.nio.file.Files;
import java.nio.file.Paths;

/**
 * Singleton with application settings
 */
public final class Settings {

    /**
     * Instance of this class.
     */
    private static Settings _instance;

    /**
     * Path to certificate file.
     */
    private String _pathToCertificate;

    /**
     * Path to file with secret key.
     */
    private String _pathToSecretKey;

    /**
     * URL of api server.
     */
    private String _apiServiceUrl;

    /**
     * This value is indicating that settings were initialized.
     */
    private boolean _initialized;

    /**
     * Private constructor.
     */
    private Settings() {
        _initialized = false;
    }

    /**
     * @return instance of this class.
     */
    public static Settings getInstance() {
        if(_instance == null) {
            _instance = new Settings();
        }

        return _instance;
    }

    /**
     * Initializing data for singleton.
     * @param pathToCertificate - Path to certificate file.
     * @param pathToSecretKey - Path to file with secret key.
     * @param apiUrl - URL of api server.
     */
    public void initialize(String pathToCertificate, String pathToSecretKey, String apiUrl) throws FileNotFoundException, NullPointerException
    {
        if (_initialized){
            return;
        }

        if (pathToCertificate.isEmpty() || Files.notExists(Paths.get(pathToCertificate))){
            throw new FileNotFoundException(pathToCertificate);
        }

        _pathToCertificate = pathToCertificate;
        if (pathToSecretKey.isEmpty() || Files.notExists(Paths.get(pathToSecretKey))){
            throw new FileNotFoundException(pathToSecretKey);
        }

        _pathToSecretKey = pathToSecretKey;
        if (apiUrl.isEmpty()){
            throw new NullPointerException(apiUrl);
        }

        _apiServiceUrl = apiUrl;
        _initialized = true;
    }

    /**
     * @return Getting path to certificate file.
     */
    public String getPathToCertificate() {
        return _pathToCertificate;
    }

    /**
     * @return Getting path to file with secret key.
     */
    public String getPathToSecretKey() {
        return _pathToSecretKey;
    }

    /**
     * @return Getting URL of api server.
     */
    public String getApiServiceUrl() {
        return _apiServiceUrl;
    }
}

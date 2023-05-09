package com.goods_account.api;

import com.goods_account.api.NativeClientRequestException;

import java.lang.IllegalArgumentException;
import java.util.IllegalFormatException;

/**
 * Response information.
 */
public class ClientResponse
{
    /**
     * Initializing of new Instance.
     * @param exception - thrown exception.
     */
    public ClientResponse(Exception exception){
        _exception = exception;
        _unautorized = false;
    }

    /**
     * Initializing of new instance.
     * @param responseCode - response error code.
     */
    public ClientResponse(int responseCode){
        _unautorized = responseCode == 401;

        try {
            _response = String.format("{ \"ErrorCode\": %n }", responseCode);
        } catch (IllegalFormatException exception){
            _response = "{ \"ErrorCode\": -1 }";
        }

        try {
            _exception = new NativeClientRequestException(responseCode);
        } catch (IllegalArgumentException exception) {
            _exception = new NativeClientRequestException();
        }
    }

    /**
     * @param body - response body.
     */
    public ClientResponse(String body){
        _response = body;
        _exception = null;
    }

    /**
     * @return response trown exception.
     */
    public Exception get_exception() {
        return _exception;
    }

    /**
     * @return result response content.
     */
    public String get_response() {
        return _response;
    }

    /**
     * @return value is indicating that response status code is 401;
     */
    public boolean unautorized(){
        return _unautorized;
    }

    /**
     * Trown exception.
     */
    private Exception _exception;

    // Content from response body or error code.
    private String _response;

    // Value is indicating that response status code is 401;
    private boolean _unautorized;
}

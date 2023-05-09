package com.goods_account.api.models.exceptions;

/**
 * Common exception for API client.
 */
public class ApiClientException extends Exception {
    /**
     * Creating new instance.
     * @param innerException is source exception.
     */
    public ApiClientException(Throwable innerException){
        super(innerException);
    }
}

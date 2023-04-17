package Services;

/**
 * This exception is throwing when server returned non 200 status code.
 */
public class NativeClientRequestException extends Exception {
    /**
     * Initializing of new instance.
     * @param statusCode Response status code.
     */
    public NativeClientRequestException(int statusCode) {
        super("Server returned response with unsuccessful status code (" + statusCode + ")");
    }

    /**
     * Initializing of new instance.
     */
    public NativeClientRequestException() {
        super("Server returned response with unsuccessful status code");
    }
}
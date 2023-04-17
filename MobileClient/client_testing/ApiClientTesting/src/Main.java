import Services.ApiClient;

public class Main {
    /**
     * Api service base url.
     */
    private static final String BaseUrl = "https://localhost:7192";

    /**
     * Path to certificate file.
     */
    private static final String CertificatePath = "I:\\Visual Studio 2017\\DemostrationProjects\\GoodsAccountingDemo\\Certificate\\server.crt";

    /**
     * Path to secret key file.
     */
    private static final String OpenKeyPath = "I:\\Visual Studio 2017\\DemostrationProjects\\GoodsAccountingDemo\\Certificate\\server.key";

    public static void main(String[] args) {
        ApiClient client = new ApiClient(BaseUrl, CertificatePath, OpenKeyPath);
        String response = client.signin("user", "Az!2.sssA");
        client.sell(1, "ed1b0587-adf1-4541-9b5f-92c9b2ea685d");
        boolean success;
        success = client.refreshToken();
        success = client.signout(1);
    }
}
package com.goods_account.data;

import com.goods_account.api.ApiClient;
import com.goods_account.api.models.exceptions.ApiClientException;
import com.goods_account.api.models.UserInfo;
import com.goods_account.api.models.exceptions.ApiUnauthorizeException;
import com.goods_account.data.model.LoggedInUser;

/**
 * Class that handles authentication w/ login credentials and retrieves user information.
 */
public class LoginDataSource {

    /**
     * Path to certificate file.
     */
    private static final String _certificatePath = "I:\\Visual Studio 2017\\DemostrationProjects\\GoodsAccountingDemo\\Certificate\\server.crt";

    /**
     * Path to secret key.
     */
    private static final String _secretKeyPath = "I:\\Visual Studio 2017\\DemostrationProjects\\GoodsAccountingDemo\\Certificate\\server.key";

    /**
     * URL to API.
     */
    private static final String _apiServiceBaseUrl = "https://localhost:7192";

    /**
     * Client for REST API instance.
     */
    private ApiClient _client;


    /**
     * Initializing new instance of data source.
     */
    public LoginDataSource(){
        _client = new ApiClient(_apiServiceBaseUrl, _certificatePath, _secretKeyPath);
    }

    /**
     * Signing on
     * @param username - User's login.
     * @param password - User's password.
     * @return - Result with response info.
     */
    public Result<LoggedInUser> login(String username, String password) {

        try {
            UserInfo response = _client.signin(username, password);
            LoggedInUser user = new LoggedInUser(response.UserId, "", response.UserDisplayedName, response.ShiftOpened, response.IsAdmin);

            return new Result.Success<LoggedInUser>(user);
        } catch (ApiClientException exception) {
            return new Result.Error(exception);
        } catch (ApiUnauthorizeException exception){
            return new Result.Error(exception);
        }
    }

    /**
     * Signing out.
     */
    public Result<Boolean> logout(int userId) {
        try {
            boolean success = _client.signout(userId);
            return new Result.Success<Boolean>(success);
        } catch (ApiClientException exception) {
            return new Result.Error(exception);
        } catch (ApiUnauthorizeException exception){
            return new Result.Error(exception);
        }
    }
}
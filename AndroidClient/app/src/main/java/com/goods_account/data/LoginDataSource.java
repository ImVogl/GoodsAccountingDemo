package com.goods_account.data;

import com.goods_account.api.ApiClient;
import com.goods_account.api.models.exceptions.ApiClientException;
import com.goods_account.api.models.UserInfo;
import com.goods_account.api.models.exceptions.ApiUnauthorizeException;
import com.goods_account.data.model.LoggedInUser;
import com.goods_account.data.model.Settings;

/**
 * Class that handles authentication w/ login credentials and retrieves user information.
 */
public class LoginDataSource {

    /**
     * Client for REST API instance.
     */
    private ApiClient _client;


    /**
     * Initializing new instance of data source.
     */
    public LoginDataSource(){
        Settings settings = Settings.getInstance();
        _client = new ApiClient(settings.getApiServiceUrl(), settings.getPathToCertificate(), settings.getPathToSecretKey());
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
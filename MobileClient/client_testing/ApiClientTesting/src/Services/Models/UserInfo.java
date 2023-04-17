package Services.Models;

import com.google.gson.annotations.SerializedName;

/**
 * Information about user.
 */
public class UserInfo extends TokenResponse
{
    /**
     * Get or set user identifier.
     */
    @SerializedName("id")
    public int UserId;


    /**
    * Get or set value in indicating that user has administrator role.
    */
    @SerializedName("is_admin")
    public boolean IsAdmin;

    /**
     * Get or set value is indicating that user has opened working shift.
     */
    @SerializedName("shift_opened")
    public boolean ShiftOpened;

    /**
     * Get or set displayed name.
     */
    @SerializedName("name")
    public String UserDisplayedName;
}

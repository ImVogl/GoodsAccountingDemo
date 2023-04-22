package com.goods_account.data.model;

/**
 * Data class that captures user information for logged in users retrieved from LoginRepository
 */
public class LoggedInUser {
    /**
     * User's identifier.
     */
    private int _userId;

    /**
     * User's email.
     */
    private String _email;

    /**
     * User's displayed name.
     */
    private String _displayName;

    /**
     * This value is indicating that user has opened working shift.
     */
    private boolean _hasOpenedWorkingShift;

    /**
     * This value is indicating that user has administrator role.
     */
    private boolean _isAdmin;

    /**
     * Initializing new instance.
     * @param userId - user's identifier.
     * @param userEmail - user's email.
     * @param displayName - user's displayed name.
     * @param shiftOpened - this value is indicating that user has opened working shift.
     */
    public LoggedInUser(
            int userId,
            String userEmail,
            String displayName,
            boolean shiftOpened,
            boolean isAdmin)
    {
        this._userId = userId;
        this._email = userEmail;
        this._displayName = displayName;
        this._hasOpenedWorkingShift = shiftOpened;
        this._isAdmin = isAdmin;
    }

    /**
     * @return Getting user's identifier.
     */
    public int get_userId() {
        return _userId;
    }

    /**
     * @return Getting user's email.
     */
    public String get_email() {
        return _email;
    }

    /**
     * @return Getting user's displayed name.
     */
    public String get_displayName() {
        return _displayName;
    }

    /**
     * @return Get value is indicating that user has opened working shift.
     */
    public boolean hasOpenedWorkingShift() {
        return _hasOpenedWorkingShift;
    }

    /**
     * @return Get value is indicating that user has administrator role.
     */
    public boolean isAdmin() {
        return _isAdmin;
    }
}
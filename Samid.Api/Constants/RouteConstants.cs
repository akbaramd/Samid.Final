namespace Samid.Api;

/// <summary>
/// Contains the route constants used across the Samid API.
/// </summary>
public static class RouteConstants
{
    #region Authentication Routes

    /// <summary>
    /// Prefix for authentication-related routes.
    /// </summary>
    public const string AuthPrefix = "/api/auth";

    /// <summary>
    /// Route for completing a user's profile.
    /// </summary>
    public const string AuthCompleteProfile = AuthPrefix + "/complete-profile";

    /// <summary>
    /// Route for retrieving a user's profile.
    /// </summary>
    public const string AuthGetProfile = AuthPrefix + "/profile";

    /// <summary>
    /// Route for sending a verification code.
    /// </summary>
    public const string AuthSendCode = AuthPrefix + "/send-code";

    /// <summary>
    /// Route for verifying a sent code.
    /// </summary>
    public const string AuthVerifyCode = AuthPrefix + "/verify-code";

    #endregion

    #region Definition Routes

    /// <summary>
    /// Prefix for definition-related routes.
    /// </summary>
    public const string DefinitionPrefix = "/api/definitions";

    /// <summary>
    /// Route for retrieving education grades.
    /// </summary>
    public const string DefinitionGetEducationGrades = DefinitionPrefix + "/grades";

    /// <summary>
    /// Route for retrieving education majors.
    /// </summary>
    public const string DefinitionGetEducationMajors = DefinitionPrefix + "/majors";

    /// <summary>
    /// Route for retrieving education stages.
    /// </summary>
    public const string DefinitionGetEducationStages = DefinitionPrefix + "/stages";

    #endregion

    #region User Routes

    /// <summary>
    /// Prefix for user-related routes.
    /// </summary>
    public const string UserPrefix = "/api/user";

    /// <summary>
    /// Route for retrieving a user's education majors.
    /// </summary>
    public const string UserGetEducationMajors = UserPrefix + "/education-majors";

    #endregion

    #region Activity Routes

    /// <summary>
    /// Prefix for activity-related routes.
    /// </summary>
    public const string ActivityPrefix = "/api/activity";

    /// <summary>
    /// Route for registering a study activity.
    /// </summary>
    public const string ActivityStudyRegister = ActivityPrefix + "/study";

    #endregion
}

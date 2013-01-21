namespace FluentACS.Specs
{
    public static class FacebookApplicationPermission
    {

        #region Email Permissions
        
        /// <summary>
        /// Provides access to the user's primary email address in the email property. Do not spam users. Your use of email must comply both with Facebook policies and with the CAN-SPAM Act.
        /// </summary>
        public const string Email = "email";

        #endregion


        #region Extended Permissions

        /// <summary>
        /// Provides access to any friend lists the user created. All user's friends are provided as part of basic data, this extended permission grants access to the lists of friends a user has created, and should only be requested if your application utilizes lists of friends.
        /// </summary>
        public const string ReadFriendlists = "read_friendlists";

        /// <summary>
        /// Provides read access to the Insights data for pages, applications, and domains the user owns.
        /// </summary>
        public const string ReadInsights = "read_insights";

        /// <summary>
        /// Provides the ability to read from a user's Facebook Inbox.
        /// </summary>
        public const string ReadMailbox = "read_mailbox";

        /// <summary>
        /// Provides read access to the user's friend requests
        /// </summary>
        public const string ReadRequests = "read_requests";

        /// <summary>
        /// Provides access to all the posts in the user's News Feed and enables your application to perform searches against the user's News Feed
        /// </summary>
        public const string ReadStream = "read_stream";

        /// <summary>
        /// Provides applications that integrate with Facebook Chat the ability to log in users.
        /// </summary>
        public const string XmppLogin = "xmpp_login";

        /// <summary>
        /// Provides the ability to manage ads and call the Facebook Ads API on behalf of a user.
        /// </summary>
        public const string AdsManagement = "ads_management";

        /// <summary>
        /// Enables your application to create and modify events on the user's behalf
        /// </summary>
        public const string CreateEvent = "create_event";

        /// <summary>
        /// Enables your app to create and edit the user's friend lists.
        /// </summary>
        public const string ManageFriendlists = "manage_friendlists";

        /// <summary>
        /// Enables your app to read notifications and mark them as read. Intended usage: This permission should be used to let users read and act on their notifications; it should not be used to for the purposes of modeling user behavior or data mining. Apps that misuse this permission may be banned from requesting it.
        /// </summary>
        public const string ManageNotifications = "manage_notifications";

        /// <summary>
        /// Provides access to the user's online/offline presence
        /// </summary>
        public const string UserOnlinePresence = "user_online_presence";

        /// <summary>
        /// Provides access to the user's friend's online/offline presence
        /// </summary>
        public const string FriendsOnlinePresense = "friends_online_presence";

        /// <summary>
        /// Enables your app to perform checkins on behalf of the user.
        /// </summary>
        public const string PublishCheckins = "publish_checkins";

        /// <summary>
        /// Enables your app to post content, comments, and likes to a user's stream and to the streams of the user's friends. This is a superset publishing permission which also includes publish_actions. However, please note that Facebook recommends a user-initiated sharing model. Please read the Platform Policies to ensure you understand how to properly use this permission. Note, you do not need to request the publish_stream permission in order to use the Feed Dialog, the Requests Dialog or the Send Dialog.
        /// </summary>
        public const string PublishStream = "publish_stream";

        /// <summary>
        /// Enables your application to RSVP to events on the user's behalf
        /// </summary>
        public const string RsvpEvent = "rsvp_event";

        #endregion


        #region Extended Profile Properties (User)

        /// <summary>
        /// Provides access to the About Me section of the profile in the about property
        /// </summary>
        public const string UserAboutMe = "user_about_me";

        /// <summary>
        /// Provides access to the user's list of activities as the activities connection
        /// </summary>
        public const string UserActivities = "user_activities";

        /// <summary>
        /// Provides access to the birthday with year as the birthday property
        /// </summary>
        public const string UserBirthday = "user_birthday";

        /// <summary>
        /// Provides read access to the authorized user's check-ins or a friend's check-ins that the user can see. This permission is superseded by user_status for new applications as of March, 2012.
        /// </summary>
        public const string UserCheckins = "user_checkins";

        /// <summary>
        /// Provides access to education history as the education property
        /// </summary>
        public const string UserEducationHistory = "user_education_history";

        /// <summary>
        /// Provides access to the list of events the user is attending as the events connection
        /// </summary>
        public const string UserEvents = "user_events";

        /// <summary>
        /// Provides access to the list of groups the user is a member of as the groups connection
        /// </summary>
        public const string UserGroups = "user_groups";

        /// <summary>
        /// Provides access to the user's hometown in the hometown property
        /// </summary>
        public const string UserHometown = "user_hometown";

        /// <summary>
        /// Provides access to the user's list of interests as the interests connection
        /// </summary>
        public const string UserInterests = "user_interests";

        /// <summary>
        /// Provides access to the list of all of the pages the user has liked as the likes connection
        /// </summary>
        public const string UserLikes = "user_likes";

        /// <summary>
        /// Provides access to the user's current location as the location property
        /// </summary>
        public const string UserLocation = "user_location";

        /// <summary>
        /// Provides access to the user's notes as the notes connection
        /// </summary>
        public const string UserNotes = "user_notes";

        /// <summary>
        /// Provides access to the photos the user has uploaded, and photos the user has been tagged in
        /// </summary>
        public const string UserPhotos = "user_photos";

        /// <summary>
        /// Provides access to the questions the user or friend has asked
        /// </summary>
        public const string UserQuestions = "user_questions";

        /// <summary>
        /// Provides access to the user's family and personal relationships and relationship status
        /// </summary>
        public const string UserRelationships = "user_relationships";

        /// <summary>
        /// Provides access to the user's relationship preferences
        /// </summary>
        public const string UserRelationshipDetails = "user_relationship_details";

        /// <summary>
        /// Provides access to the user's religious and political affiliations
        /// </summary>
        public const string UserReligionPolitics = "user_religion_politics";

        /// <summary>
        /// Provides access to the user's status messages and checkins. Please see the documentation for the location_post table for information on how this permission may affect retrieval of information about the locations associated with posts.
        /// </summary>
        public const string UserStatus = "user_status";

        /// <summary>
        /// Provides access to the user's subscribers and subscribees
        /// </summary>
        public const string UserSubscriptions = "user_subscriptions";

        /// <summary>
        /// Provides access to the videos the user has uploaded, and videos the user has been tagged in
        /// </summary>
        public const string UserVideos = "user_videos";

        /// <summary>
        /// Provides access to the user's web site URL
        /// </summary>
        public const string UserWebsite = "user_website";

        /// <summary>
        /// Provides access to work history as the work property
        /// </summary>
        public const string UserWorkHistory = "user_work_history";

        #endregion


        #region Extended Profile Properties (Friends)

        /// <summary>
        /// Provides access to the About Me section of the profile in the about property
        /// </summary>
        public const string FriendsAboutMe = "friends_about_me";

        /// <summary>
        /// Provides access to the user's list of activities as the activities connection
        /// </summary>
        public const string FriendsActivities = "friends_activities";

        /// <summary>
        /// Provides access to the birthday with year as the birthday property
        /// </summary>
        public const string FriendsBirthday = "friends_birthday";

        /// <summary>
        /// Provides read access to the authorized user's check-ins or a friend's check-ins that the user can see. This permission is superseded by friends_status for new applications as of March, 2012.
        /// </summary>
        public const string FriendsCheckins = "friends_checkins";

        /// <summary>
        /// Provides access to education history as the education property
        /// </summary>
        public const string FriendsEducationHistory = "friends_education_history";

        /// <summary>
        /// Provides access to the list of events the user is attending as the events connection
        /// </summary>
        public const string FriendsEvents = "friends_events";

        /// <summary>
        /// Provides access to the list of groups the user is a member of as the groups connection
        /// </summary>
        public const string FriendsGroups = "friends_groups";

        /// <summary>
        /// Provides access to the user's hometown in the hometown property
        /// </summary>
        public const string FriendsHometown = "friends_hometown";

        /// <summary>
        /// Provides access to the user's list of interests as the interests connection
        /// </summary>
        public const string FriendsInterests = "friends_interests";

        /// <summary>
        /// Provides access to the list of all of the pages the user has liked as the likes connection
        /// </summary>
        public const string FriendsLikes = "friends_likes";

        /// <summary>
        /// Provides access to the user's current location as the location property
        /// </summary>
        public const string FriendsLocation = "friends_location";

        /// <summary>
        /// Provides access to the user's notes as the notes connection
        /// </summary>
        public const string FriendsNotes = "friends_notes";

        /// <summary>
        /// Provides access to the photos the user has uploaded, and photos the user has been tagged in
        /// </summary>
        public const string FriendsPhotos = "friends_photos";

        /// <summary>
        /// Provides access to the questions the user or friend has asked
        /// </summary>
        public const string FriendsQuestions = "friends_questions";

        /// <summary>
        /// Provides access to the user's family and personal relationships and relationship status
        /// </summary>
        public const string FriendsRelationships = "friends_relationships";

        /// <summary>
        /// Provides access to the user's relationship preferences
        /// </summary>
        public const string FriendsRelationshipDetails = "friends_relationship_details";

        /// <summary>
        /// Provides access to the user's religious and political affiliations
        /// </summary>
        public const string FriendsReligionPolitics = "friends_religion_politics";

        /// <summary>
        /// Provides access to the user's status messages and checkins. Please see the documentation for the location_post table for information on how this permission may affect retrieval of information about the locations associated with posts.
        /// </summary>
        public const string FriendsStatus = "friends_status";

        /// <summary>
        /// Provides access to the user's subscribers and subscribees
        /// </summary>
        public const string FriendsSubscriptions = "friends_subscriptions";

        /// <summary>
        /// Provides access to the videos the user has uploaded, and videos the user has been tagged in
        /// </summary>
        public const string FriendsVideos = "friends_videos";

        /// <summary>
        /// Provides access to the user's web site URL
        /// </summary>
        public const string FriendsWebsite = "friends_website";

        /// <summary>
        /// Provides access to work history as the work property
        /// </summary>
        public const string FriendsWorkHistory = "friends_work_history";

        #endregion


        #region Open Graph Permissions (User)

        /// <summary>
        /// Allows your app to publish to the Open Graph using Built-in Actions, Achievements, Scores, or Custom Actions. Your app can also publish other activity which is detailed in the Publishing Permissions doc. Note: The user-prompt for this permission will be displayed in the first screen of the Enhanced Auth Dialog and cannot be revoked as part of the authentication flow. However, a user can later revoke this permission in their Account Settings. If you want to be notified if this happens, you should subscribe to the permissions object within the Realtime API.
        /// </summary>
        public const string PublishActions = "publish_actions";

        /// <summary>
        /// Allows you to retrieve the actions published by all applications using the built-in music.listens action.
        /// </summary>
        public const string UserActionsMusic = "user_actions.music";

        /// <summary>
        /// Allows you to retrieve the actions published by all applications using the built-in news.reads action.
        /// </summary>
        public const string UserActionsNews = "user_actions.news";

        /// <summary>
        /// Allows you to retrieve the actions published by all applications using the built-in video.watches action.
        /// </summary>
        public const string UserActionsVideo = "user_actions.video";

        /// <summary>
        /// Allows you post and retrieve game achievement activity.
        /// </summary>
        public const string UserGamesActivity = "user_games_activity";

        #endregion


        #region Open Graph Permissions (Friend)

        /// <summary>
        /// Allows you to retrieve the actions published by all applications using the built-in music.listens action.
        /// </summary>
        public const string FriendsActionsMusic = "friends_actions.music";

        /// <summary>
        /// Allows you to retrieve the actions published by all applications using the built-in news.reads action.
        /// </summary>
        public const string FriendsActionsNews = "friends_actions.news";

        /// <summary>
        /// Allows you to retrieve the actions published by all applications using the built-in video.watches action.
        /// </summary>
        public const string FriendsActionsVideo = "friends_actions.video";

        /// <summary>
        /// Allows you post and retrieve game achievement activity.
        /// </summary>
        public const string FriendsGamesActivity = "friends_games_activity";

        #endregion


        #region Page Permissions

        /// <summary>
        /// Enables your application to retrieve access_tokens for Pages and Applications that the user administrates. The access tokens can be queried by calling /<user_id>/accounts via the Graph API. See here for generating long-lived Page access tokens that do not expire after 60 days.
        /// </summary>
        public const string ManagePages = "manage_pages";

        #endregion


        #region Public Profile and Friends List

        public const string Id = "id";

        public const string Name = "name";

        public const string FirstName = "first_name";

        public const string LastName = "last_name";

        public const string Link = "link";

        public const string Username = "username";

        public const string Gender = "gender";

        public const string Locale = "locale";

        #endregion

    }
}
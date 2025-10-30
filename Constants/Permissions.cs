namespace padelya_api.Constants
{
    public static class Permissions
    {
        public static class Booking
        {
            public const string Make = "booking:make";
            public const string Create = "booking:create";
            public const string Edit = "booking:edit";
            public const string Cancel = "booking:cancel";
            public const string View = "booking:view";
            public const string ViewOwn = "booking:view_own";
            public const string AssignUser = "booking:assign_user";
            public const string MarkPaid = "booking:mark_paid";
        }

        public static class Tournament
        {
            public const string Create = "tournament:create";
            public const string Edit = "tournament:edit";
            public const string Delete = "tournament:delete";
            public const string Cancel = "tournament:cancel";
            public const string View = "tournament:view";
            public const string Join = "tournament:join";
            public const string AssignUser = "tournament:assign_user";
            public const string ManageScores = "tournament:manage_scores";
        }

        public static class Class
        {
            public const string Create = "class:create";
            public const string Edit = "class:edit";
            public const string Cancel = "class:cancel";
            public const string View = "class:view";
            public const string Join = "class:join";
            public const string AssignUser = "class:assign_user";
            public const string Leave = "class:leave";
        }

        public static class Routine
        {
            public const string Create = "routine:create";
            public const string Edit = "routine:edit";
            public const string Delete = "routine:delete";
            public const string View = "routine:view";
            public const string AssignUser = "routine:assign_user";
        }

        public static class Feedback
        {
            public const string Create = "feedback:create";
            public const string Edit = "feedback:edit";
            public const string Delete = "feedback:delete";
            public const string View = "feedback:view";
        }

        public static class User
        {
            public const string Create = "user:create";
            public const string Edit = "user:edit";
            public const string EditSelf = "user:edit_self";
            public const string View = "user:view";
            public const string AssignRoles = "user:assign_roles";
            public const string Deactivate = "user:deactivate";
        }

        public static class Role
        {
            public const string Create = "role:create";
            public const string Edit = "role:edit";
            public const string Delete = "role:delete";
            public const string PermissionAssign = "role:permission:assign";
            public const string View = "role:view";
        }
    }
}
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
      public const string Delete = "tournament:cancel";
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

    public static class Ranking
    {
      public const string ViewOwn = "ranking:view_own";
      public const string View = "ranking:view";
      public const string Manage = "ranking:manage";
    }

    public static class Product
    {
      public const string Create = "product:create";
      public const string Edit = "product:edit";
      public const string Delete = "product:delete";
      public const string View = "product:view";
    }

    public static class Category
    {
      public const string Create = "category:create";
      public const string Edit = "category:edit";
      public const string Delete = "category:delete";
      public const string View = "category:view";
    }

    public static class Order
    {
      public const string ViewAll = "order:view_all";
      public const string UpdateStatus = "order:update_status";
      public const string ViewOwn = "order:view_own";
      public const string Create = "order:create";
    }

    public static class Cart
    {
      public const string Add = "cart:add";
      public const string Remove = "cart:remove";
      public const string Update = "cart:update";
      public const string View = "cart:view";
    }
  }
}
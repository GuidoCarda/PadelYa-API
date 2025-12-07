namespace padelya_api.Constants
{
    /// <summary>
    /// Specifies the required entity type for a permission.
    /// NULL means the permission can be used by Admin (no domain profile required).
    /// Player/Teacher means the permission requires the user to have that specific domain profile.
    /// </summary>
    public enum RequiredEntityType
    {
        /// <summary>
        /// No domain profile required (Admin-only or system permissions)
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Requires a Player profile (e.g., enrollment:create_self)
        /// </summary>
        Player = 1,
        
        /// <summary>
        /// Requires a Teacher profile (e.g., attendance:create)
        /// </summary>
        Teacher = 2
    }
}


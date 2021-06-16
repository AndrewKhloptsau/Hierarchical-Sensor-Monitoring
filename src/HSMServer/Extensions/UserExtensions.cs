﻿using System.Collections.Generic;
using System.Linq;
using HSMServer.Authentication;

namespace HSMServer.Extensions
{
    internal static class UserExtensions
    {
        public static bool IsSensorAvailable(this User user, string key)
        {
            //var permissionItem = user.UserPermissions.FirstOrDefault(p => p.ProductName == server);
            //return permissionItem != null && permissionItem.IgnoredSensors.Contains(sensor);
            return user.AvailableKeys.Contains(key);
        }

        public static bool IsProductAvailable(this User user, string server)
        {
            return user.UserPermissions.FirstOrDefault(p => p.ProductName == server) != null;
        }

        public static IEnumerable<string> GetAvailableServers(this User user)
        {
            return user.UserPermissions.Select(p => p.ProductName);
        }

        public static bool IsSame(this User user, User comparedUser)
        {
            if (user == null && comparedUser == null)
            {
                return true;
            }

            if (user == null || comparedUser == null)
            {
                return false;
            }

            
            return user.UserName.Equals(comparedUser.UserName);
        }

        public static User WithoutPassword(this User user)
        {
            User copy = new User();
            copy.CertificateFileName = user.CertificateFileName;
            copy.Password = null;
            copy.CertificateThumbprint = user.CertificateThumbprint;
            copy.Role = user.Role;
            copy.UserPermissions = user.UserPermissions;
            return copy;
        }
    }
}

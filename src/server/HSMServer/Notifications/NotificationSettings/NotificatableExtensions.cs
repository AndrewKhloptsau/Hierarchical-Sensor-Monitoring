﻿using HSMServer.Authentication;
using HSMServer.Model.Authentication;
using HSMServer.Model.TreeViewModel;

namespace HSMServer.Notification.Settings
{
    internal static class NotificatableExtensions
    {
        private static readonly string[] _specialSymbolsMarkdownV2 = new[]
            {"_", "[", "]", "(", ")", "~", "`", ">", "#", "+", "-", "=", "|", "{", "}", ".", "!", "*"};

        private static string[] _escapedSymbols;


        static NotificatableExtensions()
        {
            BuildEscapedSymbols();
        }

        private static void BuildEscapedSymbols()
        {
            _escapedSymbols = new string[_specialSymbolsMarkdownV2.Length];

            for (int i = 0; i < _escapedSymbols.Length; i++)
                _escapedSymbols[i] = $"\\{_specialSymbolsMarkdownV2[i]}";
        }

        public static string EscapeMarkdownV2(this string message)
        {
            for (int i = 0; i < _escapedSymbols.Length; i++)
                message = message.Replace(_specialSymbolsMarkdownV2[i], _escapedSymbols[i]);

            return message;
        }

        internal static void UpdateEntity(this INotificatable entity, IUserManager userManager, TreeViewModel tree)
        {
            if (entity is User user)
                userManager.UpdateUser(user);
            else if (entity is ProductNodeViewModel product)
                tree.UpdateProductNotificationSettings(product);
        }

        internal static string BuildGreetings(this INotificatable entity) =>
            entity switch
            {
                User user => $"Hi, {user.Name}. ",
                ProductNodeViewModel => $"Hi. ",
                _ => string.Empty,
            };

        internal static string BuildSuccessfullResponse(this INotificatable entity) =>
            entity switch
            {
                User => "You are succesfully authorized.",
                ProductNodeViewModel product => $"Product '{product.Name}' is successfully added to group.",
                _ => string.Empty,
            };
    }
}

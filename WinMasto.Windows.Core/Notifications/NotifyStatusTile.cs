﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Mastonet.Entities;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using WinMasto.Core.Extensions;
using Notification = Mastonet.Entities.Notification;

namespace WinMasto.Core.Notifications
{
    public class NotifyStatusTile
    {
        public static void CreateStatusLiveTile(Status status)
        {
            
            var content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = CreateMediumTitleBinding(HtmlRemoval.StripTagsCharArray(status.Content), status.Account.AccountName),
                    TileWide = CreateWideTitleBinding(status.Account.AvatarUrl, HtmlRemoval.StripTagsCharArray(status.Content), status.Account.AccountName),
                    TileLarge = CreateLargeTitleBinding(status.Account.AvatarUrl, HtmlRemoval.StripTagsCharArray(status.Content), status.Account.AccountName)
                }
            };
            var tileXml = content.GetXml();
            var tileNotification = new TileNotification(tileXml);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }

        private static TileBinding CreateMediumTitleBinding(string caption, string subCaption)
        {
            var bindingContent = new TileBindingContentAdaptive()
            {
                Children =
                {
                    new AdaptiveText()
                    {
                        Text = caption.Truncate(100),
                        HintStyle = AdaptiveTextStyle.Caption
                    },
                    new AdaptiveText()
                    {
                        Text = subCaption,
                        HintWrap = true,
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    }
                }
            };
            return new TileBinding()
            {
                Branding = TileBranding.NameAndLogo,
                Content = bindingContent
            };
        }

        private static TileBinding CreateLargeTitleBinding(string imageUrl, string caption, string subCaption)
        {
            var bindingContent = new TileBindingContentAdaptive()
            {
                Children =
                {
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            new AdaptiveSubgroup()
                            {
                                HintWeight = 33,
                                Children =
                                {
                                    new AdaptiveImage()
                                    {
                                        Source = imageUrl,
                                        HintCrop = AdaptiveImageCrop.Circle
                                    }
                                }
                            },
                            new AdaptiveSubgroup()
                            {
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = caption.Truncate(100),
                                        HintStyle = AdaptiveTextStyle.Caption
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = subCaption,
                                        HintWrap = true,
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    }
                                }
                            }
                        }
                    }
                }
            };
            return new TileBinding()
            {
                Branding = TileBranding.NameAndLogo,
                Content = bindingContent
            };
        }

        private static TileBinding CreateWideTitleBinding(string imageUrl, string caption, string subCaption)
        {
            var bindingContent = new TileBindingContentAdaptive()
            {
                Children =
                {
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            new AdaptiveSubgroup()
                            {
                                HintWeight = 33,
                                Children =
                                {

                                    new AdaptiveImage()
                                    {
                                        Source = imageUrl,
                                        HintCrop = AdaptiveImageCrop.Circle
                                    }
                                }
                            },
                            new AdaptiveSubgroup()
                            {
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = caption.Truncate(100),
                                        HintStyle = AdaptiveTextStyle.Caption
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = subCaption,
                                        HintWrap = true,
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    }
                                }
                            }
                        }
                    }
                }
            };
            return new TileBinding()
            {
                Branding = TileBranding.NameAndLogo,
                Content = bindingContent
            };
        }



        public static void CreateNotificationLiveTile(Notification notification)
        {
            var notificationString = "";
            var notificationSubstring = "";
            switch (notification.Type)
            {
                case "mention":
                    notificationString = $"↩️ {notification.Account.AccountName}";
                    notificationSubstring = HtmlRemoval.StripTagsCharArray(notification.Status.Content);
                    break;
                case "reblog":
                    notificationString = $"🔁 {notification.Account.AccountName}";
                    notificationSubstring = HtmlRemoval.StripTagsCharArray(notification.Status.Content);
                    break;
                case "favourite":
                    notificationString = $"⭐ {notification.Account.AccountName} favourited your status";
                    notificationSubstring = HtmlRemoval.StripTagsCharArray(notification.Status.Content);
                    break;
                case "follow":
                    notificationString = $"New Follower:";
                    notificationSubstring = $"{notification.Account.AccountName}";
                    break;
            }
            var content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = CreateMediumTitleBinding(notificationString, notificationSubstring),
                    TileWide = CreateWideTitleBinding(notification.Account.AvatarUrl, notificationString, notificationSubstring),
                    TileLarge = CreateLargeTitleBinding(notification.Account.AvatarUrl, notificationString, notificationSubstring)
                }
            };
            var tileXml = content.GetXml();
            var tileNotification = new TileNotification(tileXml);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }

        public static ToastContent CreatePostToastContent(string imageUrl, string caption, string subCaption, Status status)
        {
            status.MediaAttachments = new List<Attachment>();
            var notification = new ToastNotificationArgs()
            {
                Type = ToastType.Status,
                Status = status
            };

            return new ToastContent()
            {
                Launch = JsonConvert.SerializeObject(notification),
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = caption
                            },
                            new AdaptiveText()
                            {
                                Text = subCaption
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = imageUrl,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }

                },
                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButton("Open Status", JsonConvert.SerializeObject(notification))
                        {
                            ActivationType = ToastActivationType.Foreground
                        },
                        new ToastButton("Sleep", JsonConvert.SerializeObject(new ToastNotificationArgs() { Type = ToastType.Sleep }))
                        {
                            ActivationType = ToastActivationType.Background
                        }
                    }
                },
                Audio = new ToastAudio()
                {
                    Src = new Uri("ms-winsoundevent:Notification.Default")
                }
            };
        }

        public static ToastContent CreateFollowToastContent(string imageUrl, string caption, Account account)
        {
            var notification = new ToastNotificationArgs()
            {
                Type = ToastType.Account,
                Account = account
            };

            return new ToastContent()
            {
                Launch = JsonConvert.SerializeObject(notification),
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = caption
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = imageUrl,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }

                },
                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButton("Open Account", JsonConvert.SerializeObject(notification))
                        {
                            ActivationType = ToastActivationType.Foreground
                        },
                        new ToastButton("Sleep", JsonConvert.SerializeObject(new ToastNotificationArgs() { Type = ToastType.Sleep }))
                        {
                            ActivationType = ToastActivationType.Background
                        }
                    }
                },
                Audio = new ToastAudio()
                {
                    Src = new Uri("ms-winsoundevent:Notification.Default")
                }
            };
        }

        public static void CreateToastNotification(Notification notification)
        {
            ToastContent content;
            switch(notification.Type)
            {
                case "mention":
                    content = CreatePostToastContent
                        (notification.Account.AvatarUrl,
                        $"{notification.Account.AccountName} mentioned you", 
                        HtmlRemoval.StripTagsCharArray(notification.Status.Content), notification.Status);
                break;
                case "reblog":
                    content = CreatePostToastContent
                    (notification.Account.AvatarUrl,
                        $"{notification.Account.AccountName} boosted your status",
                        HtmlRemoval.StripTagsCharArray(notification.Status.Content), notification.Status);
                    break;
                case "favourite":
                    content = CreatePostToastContent
                    (notification.Account.AvatarUrl,
                        $"{notification.Account.AccountName} favourited your status",
                        HtmlRemoval.StripTagsCharArray(notification.Status.Content), notification.Status);
                    break;
                case "follow":
                    content = CreateFollowToastContent
                    (notification.Account.AvatarUrl,
                        $"{notification.Account.AccountName} followed you", notification.Account);
                    break;
                default:
                    throw new Exception("Unknown type");
            }
            XmlDocument doc = content.GetXml();

            var toastNotification = new ToastNotification(doc);
            var nameProperty = toastNotification.GetType().GetRuntimeProperties().FirstOrDefault(x => x.Name == "Tag");
            nameProperty?.SetValue(toastNotification, notification.Type);
            ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
        }
    }
}

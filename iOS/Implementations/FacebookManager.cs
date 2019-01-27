﻿using DFS;
using DFS.iOS;
using DFS.Models;
using Facebook.LoginKit;
using Foundation;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

[assembly:Dependency(typeof(FacebookManager))]
namespace DFS.iOS
{
    public class FacebookManager : IFacebookManager
    {
        public Action<FacebookUser, string> _onLoginComplete;


        public void Login(Action<FacebookUser, string> onLoginComplete)
        {
            _onLoginComplete = onLoginComplete;
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }

            var tcs = new TaskCompletionSource<FacebookUser>();
            LoginManager manager = new LoginManager();
            manager.LogOut();
            manager.LoginBehavior = LoginBehavior.Native;
            manager.LogInWithReadPermissions(new string[] { "public_profile", "email" }, vc, (result, error) =>
            {
                try
                {
                    if (error != null || result == null || result.IsCancelled)
                    {
                        if (error != null)
                            _onLoginComplete?.Invoke(null, error.LocalizedDescription);
                        if (result.IsCancelled)
                            _onLoginComplete?.Invoke(null, "User Cancelled!");

                        tcs.TrySetResult(null);
                    }
                    else
                    {
                        var request = new Facebook.CoreKit.GraphRequest("me", new NSDictionary("fields", "id, first_name, email, last_name, picture.width(1000).height(1000)"));
                        request.Start((connection, result1, error1) =>
                        {
                            if (error1 != null || result1 == null)
                            {
                                Debug.WriteLine(error1.LocalizedDescription);
                                tcs.TrySetResult(null);
                            }
                            else
                            {
                                var id = string.Empty;
                                var first_name = string.Empty;
                                var email = string.Empty;
                                var last_name = string.Empty;
                                var url = string.Empty;
                                var gender = string.Empty;
                                var birthday = string.Empty;

                                try
                                {
                                    id = result1.ValueForKey(new NSString("id"))?.ToString();
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine(e.Message);
                                }

                                try
                                {
                                    first_name = result1.ValueForKey(new NSString("first_name"))?.ToString();
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine(e.Message);
                                }

                                try
                                {
                                    gender = result1.ValueForKey(new NSString("gender"))?.ToString();
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine(e.Message);
                                }

                                try
                                {
                                    gender = result1.ValueForKey(new NSString("birthday"))?.ToString();
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine(e.Message);
                                }

                                try
                                {
                                    email = result1.ValueForKey(new NSString("email"))?.ToString();
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine(e.Message);
                                }

                                try
                                {
                                    last_name = result1.ValueForKey(new NSString("last_name"))?.ToString();
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine(e.Message);
                                }

                                try
                                {
                                    url = ((result1.ValueForKey(new NSString("picture")) as NSDictionary).ValueForKey(new NSString("data")) as NSDictionary).ValueForKey(new NSString("url")).ToString();
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine(e.Message);
                                }
                                if (tcs != null)
                                {
                                    tcs.TrySetResult(new FacebookUser(id, result.Token.TokenString, first_name, last_name, email, url, gender, birthday));
                                    _onLoginComplete?.Invoke(new FacebookUser(id, result.Token.TokenString, first_name, last_name, email, url, gender, birthday), string.Empty);
                                }
                            }
                        });
                    }
                }
                catch(Exception ex) 
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            });
        }

        public void Logout()
        {
            LoginManager manager = new LoginManager();
            manager.LogOut();
        }
    }
}
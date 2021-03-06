﻿using BLL;
using BLL.ViewModel;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InvMe.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HashtagsPage : ContentPage
	{
        #region Properties

        List<Hashtags> Bindablehashtags = new List<Hashtags>();

        private bool isOnline = false;
        
        #endregion
        
        public HashtagsPage()
        {
            InitializeComponent();
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();

            GetHashtags();
        }

        private void GetHashtags()
        {
            Bindablehashtags = new List<Hashtags>();

            Bindablehashtags = new HashtagsPageViewModel().GetHashtagsByUserID();
            
            hashtagsListView.ItemsSource = Bindablehashtags;

            if (Bindablehashtags.Count != 0)
            {
                joinedEventNoItemLabel.IsVisible = false;
            }
            else
            {
                joinedEventNoItemLabel.IsVisible = true;
            }
        }
        
        private void onlineSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            isOnline = !isOnline;
            townEntry.IsVisible = !townEntry.IsVisible;
            townImage.IsVisible = !townImage.IsVisible;
        }
        
        private void DidableEnableButton(bool state)
        {
            addHashtagActivator.IsRunning = !state;
            addHashtagActivator.IsVisible = !state;
            addButton.IsEnabled = state;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            DidableEnableButton(false);

            Hashtags hashtag = new Hashtags();

            hashtag.UID = GlobalVariables.ActualUser.ID;

            if (isOnline)
            {
                hashtag.TOWN = "Online";
            }
            else
            {
                if (!string.IsNullOrEmpty(townEntry.Text))
                {
                    hashtag.TOWN = townEntry.Text;
                }
                else
                {
                    await DisplayAlert(GlobalVariables.Language.Warning(), GlobalVariables.Language.FillTheHashtagEntry(), GlobalVariables.Language.OK());

                    DidableEnableButton(true);

                    return;
                }
            }

            if (string.IsNullOrEmpty(hashtagEntry.Text))
            {
                await DisplayAlert(GlobalVariables.Language.Warning(), GlobalVariables.Language.FillTheTownEntry(), GlobalVariables.Language.OK());

                DidableEnableButton(true);

                return;
            }
            else
            {
                hashtag.HASHTAG = hashtagEntry.Text;

                string success = new HashtagsPageViewModel().InsertHashtag(hashtag);

                if (string.IsNullOrEmpty(success))
                {
                    townEntry.Text = string.Empty;
                    hashtagEntry.Text = string.Empty;

                    HashtagsListView_Refreshing(null, new EventArgs());
                }
                else
                {
                    await DisplayAlert(GlobalVariables.Language.Warning(), success, GlobalVariables.Language.OK());
                }
            }

            DidableEnableButton(true);
        }

        private void HashtagsListView_Refreshing(object sender, EventArgs e)
        {
            hashtagsListView.IsRefreshing = true;

            GetHashtags();

            hashtagsListView.IsRefreshing = false;
        }

        private void HashtagsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var listItem = (Hashtags)hashtagsListView.SelectedItem;

            Navigation.PushAsync(new HomePage(listItem));
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            DidableEnableButton(false);
            
            string success = new HashtagsPageViewModel().DeleteHashtag((Hashtags)((Image)sender).BindingContext);

            if (string.IsNullOrEmpty(success))
            {
                HashtagsListView_Refreshing(null, new EventArgs());
            }
            else
            {
                await DisplayAlert(GlobalVariables.Language.Warning(), success, GlobalVariables.Language.OK());
            }

            DidableEnableButton(true);
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Core;
using Sonos.Client;
using SmartHomeController.Server;
using System.Net.Http;
using Sonos.Client.Models;
using System.Xml.Linq;
using Windows.UI.Xaml.Media.Imaging;
using System.Net;
using Windows.Networking.Connectivity;
using SmartHome.Client;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SmartHomeController
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private HttpServer server;
        private SonosClient sonosClient;
        private SmartHome.Client.SmartHomeClient smartHomeClient;

        public MainPage()
        {
            this.InitializeComponent();

            SmartHomeClient.Config.CloudVersion = SmartHome.Client.Rest.CloudVersion.Staging;
            SmartHomeClient.Config.ClientId = "smart_home_controller";
            SmartHomeClient.Config.ClientSecret = "vZq1tXMMJUJJ1ThIOkTl+TG2YS19waSw+P6MsY#DdFCR5o7TM+wreiubT3dalSey";
        }

        private string GetIpAddress()
        {
            List<string> ipAddresses = new List<string>();
            var hostnames = NetworkInformation.GetHostNames();
            foreach (var hn in hostnames)
            {
                //IanaInterfaceType == 71 => Wifi
                //IanaInterfaceType == 6 => Ethernet (Emulator)
                if (hn.IPInformation != null &&
                   (hn.IPInformation.NetworkAdapter.IanaInterfaceType == 71
                   || hn.IPInformation.NetworkAdapter.IanaInterfaceType == 6))
                {
                    string ipAddress = hn.DisplayName;
                    ipAddresses.Add(ipAddress);
                }
            }
            if (ipAddresses.Any())
                return ipAddresses.FirstOrDefault();
            else
                return "";
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string localIpAddress = GetIpAddress();
            int localPort = 12345;


            //Task.Delay(5000);

            //const int port = 12345;
            //using (HttpServer server = new HttpServer(port))
            //{
            //    using (HttpClient client = new HttpClient())
            //    {
            //        try
            //        {
            //            byte[] data = await client.GetByteArrayAsync(
            //                          "http://localhost:" + port + "/foo.txt");

            //            // do something with 
            //        }
            //        catch (HttpRequestException)
            //        {
            //            // possibly a 404
            //        }
            //    }
            //}

            try
            {
                sonosClient = new SonosClient(AppSettings.Instance.SonosIP);
                sonosClient.NotificationEvent += SonosClient_NotificationEvent;

                server = new HttpServer(localPort, sonosClient);

                //await sonosClient.SubscribeToZoneGroupTopology(localIpAddress, localPort);

                var t = await sonosClient.Subscribe(localIpAddress, localPort);

                //smartHomeClient = new SmartHome.Client.SmartHomeClient();
                //SmartHome.Client.Models.SmartHomeUser user = new SmartHome.Client.Models.SmartHomeUser("mail@sebastianroesch.de");
                //user = await smartHomeClient.RegisterUserAsync(user);


#if DEBUG
                //TimeSpan debugPeriod = TimeSpan.FromMilliseconds(5000);
                //ThreadPoolTimer DebugTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
                //{
                //    await Dispatcher.RunAsync(CoreDispatcherPriority.High,
                //        async () =>
                //        {
                //            //await sonosClient.ParseNotification("<e:propertyset xmlns:e=\"urn:schemas-upnp-org:event-1-0\"><e:property><LastChange>&lt;Event xmlns=&quot;urn:schemas-upnp-org:metadata-1-0/AVT/&quot; xmlns:r=&quot;urn:schemas-rinconnetworks-com:metadata-1-0/&quot;&gt;&lt;InstanceID val=&quot;0&quot;&gt;&lt;TransportState val=&quot;PLAYING&quot;/&gt;&lt;CurrentPlayMode val=&quot;NORMAL&quot;/&gt;&lt;CurrentCrossfadeMode val=&quot;0&quot;/&gt;&lt;NumberOfTracks val=&quot;43&quot;/&gt;&lt;CurrentTrack val=&quot;43&quot;/&gt;&lt;CurrentSection val=&quot;0&quot;/&gt;&lt;CurrentTrackURI val=&quot;x-sonos-spotify:spotify%3atrack%3a41Fflg7qHiVOD6dEPvsCzO?sid=9&amp;amp;flags=32&amp;amp;sn=2&quot;/&gt;&lt;CurrentTrackDuration val=&quot;0:03:45&quot;/&gt;&lt;CurrentTrackMetaData val=&quot;&amp;lt;DIDL-Lite xmlns:dc=&amp;quot;http://purl.org/dc/elements/1.1/&amp;quot; xmlns:upnp=&amp;quot;urn:schemas-upnp-org:metadata-1-0/upnp/&amp;quot; xmlns:r=&amp;quot;urn:schemas-rinconnetworks-com:metadata-1-0/&amp;quot; xmlns=&amp;quot;urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/&amp;quot;&amp;gt;&amp;lt;item id=&amp;quot;-1&amp;quot; parentID=&amp;quot;-1&amp;quot; restricted=&amp;quot;true&amp;quot;&amp;gt;&amp;lt;res protocolInfo=&amp;quot;sonos.com-spotify:*:audio/x-spotify:*&amp;quot; duration=&amp;quot;0:03:45&amp;quot;&amp;gt;x-sonos-spotify:spotify%3atrack%3a41Fflg7qHiVOD6dEPvsCzO?sid=9&amp;amp;amp;flags=32&amp;amp;amp;sn=2&amp;lt;/res&amp;gt;&amp;lt;r:streamContent&amp;gt;&amp;lt;/r:streamContent&amp;gt;&amp;lt;r:radioShowMd&amp;gt;&amp;lt;/r:radioShowMd&amp;gt;&amp;lt;upnp:albumArtURI&amp;gt;/getaa?s=1&amp;amp;amp;u=x-sonos-spotify%3aspotify%253atrack%253a41Fflg7qHiVOD6dEPvsCzO%3fsid%3d9%26flags%3d32%26sn%3d2&amp;lt;/upnp:albumArtURI&amp;gt;&amp;lt;dc:title&amp;gt;Worth It&amp;lt;/dc:title&amp;gt;&amp;lt;upnp:class&amp;gt;object.item.audioItem.musicTrack&amp;lt;/upnp:class&amp;gt;&amp;lt;dc:creator&amp;gt;Fifth Harmony&amp;lt;/dc:creator&amp;gt;&amp;lt;upnp:album&amp;gt;Reflection (Deluxe)&amp;lt;/upnp:album&amp;gt;&amp;lt;/item&amp;gt;&amp;lt;/DIDL-Lite&amp;gt;&quot;/&gt;&lt;r:NextTrackURI val=&quot;&quot;/&gt;&lt;r:NextTrackMetaData val=&quot;&quot;/&gt;&lt;r:EnqueuedTransportURI val=&quot;file:///jffs/settings/trackqueue.rsq#0&quot;/&gt;&lt;r:EnqueuedTransportURIMetaData val=&quot;&quot;/&gt;&lt;PlaybackStorageMedium val=&quot;NETWORK&quot;/&gt;&lt;AVTransportURI val=&quot;x-rincon-queue:RINCON_B8E937EABEFA01400#0&quot;/&gt;&lt;AVTransportURIMetaData val=&quot;&quot;/&gt;&lt;NextAVTransportURI val=&quot;&quot;/&gt;&lt;NextAVTransportURIMetaData val=&quot;&quot;/&gt;&lt;CurrentTransportActions val=&quot;Set, Play, Stop, Pause, Seek, Next, Previous&quot;/&gt;&lt;r:CurrentValidPlayModes val=&quot;SHUFFLE, REPEAT, CROSSFADE&quot;/&gt;&lt;TransportStatus val=&quot;OK&quot;/&gt;&lt;r:SleepTimerGeneration val=&quot;0&quot;/&gt;&lt;r:AlarmRunning val=&quot;0&quot;/&gt;&lt;r:SnoozeRunning val=&quot;0&quot;/&gt;&lt;r:RestartPending val=&quot;0&quot;/&gt;&lt;TransportPlaySpeed val=&quot;NOT_IMPLEMENTED&quot;/&gt;&lt;CurrentMediaDuration val=&quot;NOT_IMPLEMENTED&quot;/&gt;&lt;RecordStorageMedium val=&quot;NOT_IMPLEMENTED&quot;/&gt;&lt;PossiblePlaybackStorageMedia val=&quot;NONE, NETWORK&quot;/&gt;&lt;PossibleRecordStorageMedia val=&quot;NOT_IMPLEMENTED&quot;/&gt;&lt;RecordMediumWriteStatus val=&quot;NOT_IMPLEMENTED&quot;/&gt;&lt;CurrentRecordQualityMode val=&quot;NOT_IMPLEMENTED&quot;/&gt;&lt;PossibleRecordQualityModes val=&quot;NOT_IMPLEMENTED&quot;/&gt;&lt;/InstanceID&gt;&lt;/Event&gt;</LastChange></e:property></e:propertyset>");
                //        });
                //}, debugPeriod);
#endif

                TimeSpan period = TimeSpan.FromMilliseconds(100);
                ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
                {
                    // Update the UI thread by using the UI core dispatcher.
                    await Dispatcher.RunAsync(CoreDispatcherPriority.High,
                        () =>
                        {
                            if (!Paused)
                            {
                                if (!LastTimerRunTime.HasValue)
                                    LastTimerRunTime = DateTime.UtcNow;

                                if (ElapsedTrackTime != null && TotalTrackTime != null && LastTimerRunTime != null)
                                {
                                    ElapsedTrackTime += DateTime.UtcNow - LastTimerRunTime.Value;
                                    if (TotalTrackTime.TotalMilliseconds > 0)
                                    {
                                        double percentage = ElapsedTrackTime.TotalMilliseconds / TotalTrackTime.TotalMilliseconds;
                                        TrackProgress.Value = percentage * 100;
                                    }
                                    LastTimerRunTime = DateTime.UtcNow;
                                }
                            }
                        });
                }, period);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private DateTime? LastTimerRunTime;
        private TimeSpan TotalTrackTime;
        private TimeSpan ElapsedTrackTime;
        private string CurrentTrackUri;
        private bool Paused = true;
        private bool Muted = false;
        private int Volume = 0;

        private async void SonosClient_NotificationEvent(object sender, Event e)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            await Dispatcher.RunAsync(CoreDispatcherPriority.High,
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                async () =>
                {
                    try
                    {
                        try
                        {
                            PositionInfoResponse positionInfo = await sonosClient.GetPositionInfo();
                            if (positionInfo != null)
                            {
                                ElapsedTrackTime = positionInfo.ElapsedTime;
                                double percentage = ElapsedTrackTime.TotalMilliseconds / TotalTrackTime.TotalMilliseconds;
                                TrackProgress.Value = percentage * 100;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }

                        if (e != null && e.InstanceID != null)
                        {
                            if (e.InstanceID.CurrentTrackDuration != null)
                                TotalTrackTime = e.InstanceID.CurrentTrackDuration.Duration;

                            // Current and previous album cover
                            if (e.InstanceID.CurrentTrackMetaData != null && e.InstanceID.CurrentTrackMetaData.TrackMeta != null && e.InstanceID.CurrentTrackMetaData.TrackMeta.Item != null)
                            {
                                TrackName.Text = e.InstanceID.CurrentTrackMetaData.TrackMeta.Item.Title;
                                ArtistName.Text = e.InstanceID.CurrentTrackMetaData.TrackMeta.Item.Creator;

                                var spotifyCover = await sonosClient.GetTrackInfo(e.InstanceID.CurrentTrackURI);

                                var currentCoverImageUri = string.Format("http://{0}:1400{1}", AppSettings.Instance.SonosIP, WebUtility.UrlDecode(e.InstanceID.CurrentTrackMetaData.TrackMeta.Item.AlbumArtURIClean));
                                var currentCoverImageSrc = (CoverImage.Source as BitmapImage).UriSource.ToString();
                                var prevcoverimagesrc = "";
                                if (PrevCoverImage.Source != null)
                                    prevcoverimagesrc = (PrevCoverImage.Source as BitmapImage).UriSource.ToString();

                                if (currentCoverImageSrc != currentCoverImageUri)
                                {
                                    if (currentCoverImageSrc == "ms-appx:///Assets/Icons/Music.png")
                                        PrevCoverImage.Source = null;
                                    else
                                        PrevCoverImage.Source = CoverImage.Source;

                                    CoverImage.Source = new BitmapImage(new Uri(currentCoverImageUri, UriKind.Absolute));
                                }
                            }

                            // Next album cover
                            if (e.InstanceID.NextTrackMetaData != null && e.InstanceID.NextTrackMetaData.TrackMeta != null && e.InstanceID.NextTrackMetaData.TrackMeta.Item != null)
                            {
                                var nextCoverImageUri = string.Format("http://{0}:1400{1}", AppSettings.Instance.SonosIP, WebUtility.UrlDecode(e.InstanceID.NextTrackMetaData.TrackMeta.Item.AlbumArtURIClean));

                                var nextCoverImageSrc = "";
                                if (NextCoverImage.Source != null)
                                    nextCoverImageSrc = (NextCoverImage.Source as BitmapImage).UriSource.ToString();

                                if (nextCoverImageSrc != nextCoverImageUri)
                                    NextCoverImage.Source = new BitmapImage(new Uri(nextCoverImageUri, UriKind.Absolute));
                            }

                            if (e.InstanceID.TransportState != null && e.InstanceID.TransportState.Playing)
                            {
                                ImageBrush background = new ImageBrush();
                                background.ImageSource = new BitmapImage(new Uri(@"ms-appx:///Assets/Icons/Pause.png", UriKind.Absolute));
                                PlayButton.Background = background;
                                Paused = false;
                            }
                            else
                            {
                                ImageBrush background = new ImageBrush();
                                background.ImageSource = new BitmapImage(new Uri(@"ms-appx:///Assets/Icons/Play.png", UriKind.Absolute));
                                PlayButton.Background = background;
                                Paused = true;
                            }

                            if (e.InstanceID.Volume.Any())
                            {
                                var masterVolume = e.InstanceID.Volume.Where(v => v.Channel == "Master").FirstOrDefault();
                                if (masterVolume != null)
                                {
                                    Volume = masterVolume.VolumeValue;
                                    VolumeSlider.Value = masterVolume.VolumeValue;
                                }
                            }

                            if (e.InstanceID.Mute.Any())
                            {
                                var masterVolume = e.InstanceID.Mute.Where(v => v.Channel == "Master").FirstOrDefault();
                                if (masterVolume != null)
                                {
                                    Muted = masterVolume.MuteValue;
                                    if (Muted)
                                    {
                                        ImageBrush background = new ImageBrush();
                                        background.ImageSource = new BitmapImage(new Uri(@"ms-appx:///Assets/Icons/Volume-Mute.png", UriKind.Absolute));
                                        MuteButton.Background = background;
                                    }
                                    else
                                    {
                                        ImageBrush background = new ImageBrush();
                                        background.ImageSource = new BitmapImage(new Uri(@"ms-appx:///Assets/Icons/Volume-On.png", UriKind.Absolute));
                                        MuteButton.Background = background;
                                    }
                                }
                            }

                            if (e.InstanceID.CurrentTrackURI != null)
                            {
                                if (CurrentTrackUri != e.InstanceID.CurrentTrackURI.Val)
                                {
                                    ElapsedTrackTime = new TimeSpan();
                                    LastTimerRunTime = DateTime.UtcNow;
                                    TrackProgress.Value = 0;


                                    // Send context update
                                    SmartHome.Client.Models.UserContextUpdate update = new SmartHome.Client.Models.UserContextUpdate("home.devices.soundsystems.play", "", "home.device.status");
                                    update.Parameters.Add("DeviceId", (await sonosClient.GetDeviceDescription()).Device.UDN);
                                    update.Parameters.Add("SongUri", e.InstanceID.CurrentTrackURI.Val);
                                    update.Parameters.Add("Volume", Volume + "");

                                    if (Paused)
                                        update.ContextKey = "home.devices.soundsystems.play";
                                    else
                                        update.ContextKey = "home.devices.soundsystems.pause";

                                    await smartHomeClient.UpdateContextAsync(update);
                                }
                                CurrentTrackUri = e.InstanceID.CurrentTrackURI.Val;
                            }
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                });
        }

        private async void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (sonosClient != null)
            {
                await sonosClient.Previous();
                ElapsedTrackTime = new TimeSpan();
                LastTimerRunTime = DateTime.UtcNow;
            }
        }

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (sonosClient != null)
            {
                var playing = await sonosClient.IsPlaying();
                if (playing)
                {
                    await sonosClient.Pause();
                    Paused = true;
                }
                else
                {
                    await sonosClient.Play();
                    Paused = false;
                }
            }
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (sonosClient != null)
            {
                await sonosClient.Next();
                ElapsedTrackTime = new TimeSpan();
                LastTimerRunTime = DateTime.UtcNow;
            }
        }

        private void VolumeButton_Click(object sender, RoutedEventArgs e)
        {
            if (VolumePopup.Visibility == Visibility.Collapsed)
                VolumePopup.Visibility = Visibility.Visible;
            else
                VolumePopup.Visibility = Visibility.Collapsed;
        }

        private async void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            //Todo: remove volume limit
            if (e.NewValue != Volume && e.NewValue < 40)
            {
                Volume = (int)e.NewValue;
                await sonosClient.SetVolume((int)e.NewValue);
            }
        }

        private async void MuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Muted)
            {
                await sonosClient.SetMute(false);
                Muted = false;
            } else
            {
                await sonosClient.SetMute(true);
                Muted = true;
            }
        }
    }
}

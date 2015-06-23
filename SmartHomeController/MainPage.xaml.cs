using System;
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

        public MainPage()
        {
            this.InitializeComponent();

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

                var t = await sonosClient.Subscribe(localIpAddress, localPort);

                TimeSpan debugPeriod = TimeSpan.FromMilliseconds(5000);
                ThreadPoolTimer DebugTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.High,
                        async () =>
                        {
                            //await sonosClient.ParseNotification("<e:propertyset xmlns:e=\"urn:schemas-upnp-org:event-1-0\"><e:property><LastChange>&lt;Event xmlns=&quot;urn:schemas-upnp-org:metadata-1-0/AVT/&quot; xmlns:r=&quot;urn:schemas-rinconnetworks-com:metadata-1-0/&quot;&gt;&lt;InstanceID val=&quot;0&quot;&gt;&lt;TransportState val=&quot;PLAYING&quot;/&gt;&lt;CurrentPlayMode val=&quot;NORMAL&quot;/&gt;&lt;CurrentCrossfadeMode val=&quot;0&quot;/&gt;&lt;NumberOfTracks val=&quot;43&quot;/&gt;&lt;CurrentTrack val=&quot;43&quot;/&gt;&lt;CurrentSection val=&quot;0&quot;/&gt;&lt;CurrentTrackURI val=&quot;x-sonos-spotify:spotify%3atrack%3a41Fflg7qHiVOD6dEPvsCzO?sid=9&amp;amp;flags=32&amp;amp;sn=2&quot;/&gt;&lt;CurrentTrackDuration val=&quot;0:03:45&quot;/&gt;&lt;CurrentTrackMetaData val=&quot;&amp;lt;DIDL-Lite xmlns:dc=&amp;quot;http://purl.org/dc/elements/1.1/&amp;quot; xmlns:upnp=&amp;quot;urn:schemas-upnp-org:metadata-1-0/upnp/&amp;quot; xmlns:r=&amp;quot;urn:schemas-rinconnetworks-com:metadata-1-0/&amp;quot; xmlns=&amp;quot;urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/&amp;quot;&amp;gt;&amp;lt;item id=&amp;quot;-1&amp;quot; parentID=&amp;quot;-1&amp;quot; restricted=&amp;quot;true&amp;quot;&amp;gt;&amp;lt;res protocolInfo=&amp;quot;sonos.com-spotify:*:audio/x-spotify:*&amp;quot; duration=&amp;quot;0:03:45&amp;quot;&amp;gt;x-sonos-spotify:spotify%3atrack%3a41Fflg7qHiVOD6dEPvsCzO?sid=9&amp;amp;amp;flags=32&amp;amp;amp;sn=2&amp;lt;/res&amp;gt;&amp;lt;r:streamContent&amp;gt;&amp;lt;/r:streamContent&amp;gt;&amp;lt;r:radioShowMd&amp;gt;&amp;lt;/r:radioShowMd&amp;gt;&amp;lt;upnp:albumArtURI&amp;gt;/getaa?s=1&amp;amp;amp;u=x-sonos-spotify%3aspotify%253atrack%253a41Fflg7qHiVOD6dEPvsCzO%3fsid%3d9%26flags%3d32%26sn%3d2&amp;lt;/upnp:albumArtURI&amp;gt;&amp;lt;dc:title&amp;gt;Worth It&amp;lt;/dc:title&amp;gt;&amp;lt;upnp:class&amp;gt;object.item.audioItem.musicTrack&amp;lt;/upnp:class&amp;gt;&amp;lt;dc:creator&amp;gt;Fifth Harmony&amp;lt;/dc:creator&amp;gt;&amp;lt;upnp:album&amp;gt;Reflection (Deluxe)&amp;lt;/upnp:album&amp;gt;&amp;lt;/item&amp;gt;&amp;lt;/DIDL-Lite&amp;gt;&quot;/&gt;&lt;r:NextTrackURI val=&quot;&quot;/&gt;&lt;r:NextTrackMetaData val=&quot;&quot;/&gt;&lt;r:EnqueuedTransportURI val=&quot;file:///jffs/settings/trackqueue.rsq#0&quot;/&gt;&lt;r:EnqueuedTransportURIMetaData val=&quot;&quot;/&gt;&lt;PlaybackStorageMedium val=&quot;NETWORK&quot;/&gt;&lt;AVTransportURI val=&quot;x-rincon-queue:RINCON_B8E937EABEFA01400#0&quot;/&gt;&lt;AVTransportURIMetaData val=&quot;&quot;/&gt;&lt;NextAVTransportURI val=&quot;&quot;/&gt;&lt;NextAVTransportURIMetaData val=&quot;&quot;/&gt;&lt;CurrentTransportActions val=&quot;Set, Play, Stop, Pause, Seek, Next, Previous&quot;/&gt;&lt;r:CurrentValidPlayModes val=&quot;SHUFFLE, REPEAT, CROSSFADE&quot;/&gt;&lt;TransportStatus val=&quot;OK&quot;/&gt;&lt;r:SleepTimerGeneration val=&quot;0&quot;/&gt;&lt;r:AlarmRunning val=&quot;0&quot;/&gt;&lt;r:SnoozeRunning val=&quot;0&quot;/&gt;&lt;r:RestartPending val=&quot;0&quot;/&gt;&lt;TransportPlaySpeed val=&quot;NOT_IMPLEMENTED&quot;/&gt;&lt;CurrentMediaDuration val=&quot;NOT_IMPLEMENTED&quot;/&gt;&lt;RecordStorageMedium val=&quot;NOT_IMPLEMENTED&quot;/&gt;&lt;PossiblePlaybackStorageMedia val=&quot;NONE, NETWORK&quot;/&gt;&lt;PossibleRecordStorageMedia val=&quot;NOT_IMPLEMENTED&quot;/&gt;&lt;RecordMediumWriteStatus val=&quot;NOT_IMPLEMENTED&quot;/&gt;&lt;CurrentRecordQualityMode val=&quot;NOT_IMPLEMENTED&quot;/&gt;&lt;PossibleRecordQualityModes val=&quot;NOT_IMPLEMENTED&quot;/&gt;&lt;/InstanceID&gt;&lt;/Event&gt;</LastChange></e:property></e:propertyset>");
                        });
                }, debugPeriod);

                TimeSpan period = TimeSpan.FromMilliseconds(100);
                ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
                {
                    await

                                        //var playing = await sonosClient.IsPlaying();
                                        //var volume = await sonosClient.GetVolume();
                                        // 
                                        // Update the UI thread by using the UI core dispatcher.
                                        // 
                                        Dispatcher.RunAsync(CoreDispatcherPriority.High,
                                            () =>
                                            {
                                                if (!Paused)
                                                {
                                                    if (!LastTimerRunTime.HasValue)
                                                        LastTimerRunTime = DateTime.UtcNow;

                                                    if (ElapsedTrackTime != null && TotalTrackTime != null && LastTimerRunTime != null)
                                                    {
                                                        ElapsedTrackTime += DateTime.UtcNow - LastTimerRunTime.Value;

                                                        double percentage = ElapsedTrackTime.TotalMilliseconds / TotalTrackTime.TotalMilliseconds;

                                                        TrackProgress.Value = percentage * 100;

                                                        LastTimerRunTime = DateTime.UtcNow;
                                                    }
                                                }


                                                // 
                                                // UI components can be accessed within this scope.
                                                // 

                                                //VolumeValue.Text = volume + "";
                                                //PlayingValue.Text = playing ? "PLAYING" : "PAUSED";

                                            });
                },
                    period,
                    async (source) =>
                    {
                        // 
                        // TODO: Handle periodic timer cancellation.
                        // 

                        // 
                        // Update the UI thread by using the UI core dispatcher.
                        // 
                        await Dispatcher.RunAsync(CoreDispatcherPriority.High,
                            async () =>
                                            {
                                                // 
                                                // UI components can be accessed within this scope.
                                                //                 

                                                // Periodic timer cancelled.

                                            });
                    }
            );



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

        private void SonosClient_NotificationEvent(object sender, Event e)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Dispatcher.RunAsync(CoreDispatcherPriority.High,
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                           () =>
                                           {
                                               try
                                               {


                                                   if (CurrentTrackUri != e.InstanceID.CurrentTrackURI.Val)
                                                   {
                                                       //TrackStartedTime = DateTime.UtcNow;
                                                       ElapsedTrackTime = new TimeSpan();
                                                       LastTimerRunTime = DateTime.UtcNow;
                                                       TrackProgress.Value = 0;
                                                   }

                                                   CurrentTrackUri = e.InstanceID.CurrentTrackURI.Val;
                                                   TotalTrackTime = e.InstanceID.CurrentTrackDuration.Duration;

                                                   PlayingValue.Text = e.InstanceID.TransportState.Val;
                                                   TrackName.Text = e.InstanceID.CurrentTrackMetaData.TrackMeta.Item.Title;
                                                   ArtistName.Text = e.InstanceID.CurrentTrackMetaData.TrackMeta.Item.Creator;

                                                   var currentImageUri = string.Format("http://{0}:1400{1}", AppSettings.Instance.SonosIP, WebUtility.UrlDecode(e.InstanceID.CurrentTrackMetaData.TrackMeta.Item.AlbumArtURIClean));
                                                   var nextImageUri = string.Format("http://{0}:1400{1}", AppSettings.Instance.SonosIP, WebUtility.UrlDecode(e.InstanceID.NextTrackMetaData.TrackMeta.Item.AlbumArtURIClean));
                                                   var coverimagesrc = (CoverImage.Source as BitmapImage).UriSource.ToString();
                                                   var nextimagesrc = "";
                                                   if(NextCoverImage.Source != null)
                                                       nextimagesrc = (NextCoverImage.Source as BitmapImage).UriSource.ToString();
                                                   var prevcoverimagesrc = "";
                                                   if (PrevCoverImage.Source != null)
                                                   {
                                                       prevcoverimagesrc = (PrevCoverImage.Source as BitmapImage).UriSource.ToString();
                                                   }


                                                   if (coverimagesrc != currentImageUri)
                                                   {
                                                       if (coverimagesrc == "ms-appx:///Assets/Icons/Music.png")
                                                           PrevCoverImage.Source = null;
                                                       else
                                                           PrevCoverImage.Source = CoverImage.Source;

                                                       CoverImage.Source = new BitmapImage(new Uri(currentImageUri, UriKind.Absolute));
                                                   }
                                                   if (nextimagesrc != nextImageUri)
                                                   {
                                                       NextCoverImage.Source = new BitmapImage(new Uri(nextImageUri, UriKind.Absolute));
                                                   }


                                                   if (e.InstanceID.TransportState.Playing)
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
    }
}

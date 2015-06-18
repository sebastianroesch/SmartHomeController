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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SmartHomeController
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                SonosLocalClient.SonosClient sonosClient = new SonosLocalClient.SonosClient();

                TimeSpan period = TimeSpan.FromMilliseconds(300);

                ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
                {
                    // 
                    // Work
                    // 
                    var playing = await sonosClient.IsPlaying();
                    var volume = await sonosClient.GetVolume();
                    // 
                    // Update the UI thread by using the UI core dispatcher.
                    // 
                    Dispatcher.RunAsync(CoreDispatcherPriority.High,
                        () =>
                        {
                            // 
                            // UI components can be accessed within this scope.
                            // 

                            VolumeValue.Text = volume + "";
                            PlayingValue.Text = playing ? "PLAYING" : "PAUSED";

                        });
                },
                    period,
                    (source) =>
                    {
                        // 
                        // TODO: Handle periodic timer cancellation.
                        // 

                        // 
                        // Update the UI thread by using the UI core dispatcher.
                        // 
                        Dispatcher.RunAsync(CoreDispatcherPriority.High,
                            () =>
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


    }
}

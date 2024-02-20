using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace Colosoft.Presentation
{
    public static class UIHelper2
    {
        private const int MaxBrowserRequests = 3;
        private static int launchBrowserRequests;

        static UIHelper2()
        {
            EventManager.RegisterClassHandler(typeof(Hyperlink), Hyperlink.ClickEvent, new RoutedEventHandler(OnHyperlinkClick));
        }

        public static void WaitForPriority(DispatcherPriority priority)
        {
            Dispatcher.CurrentDispatcher.WaitForPriority(priority);
        }

        public static void WaitForPriority(this Dispatcher dispatcher, DispatcherPriority priority)
        {
            if (dispatcher is null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            var frame = new DispatcherFrame();
            var dispatcherOperation = dispatcher.BeginInvoke(priority, new DispatcherOperationCallback(ExitFrameOperation), frame);
            Dispatcher.PushFrame(frame);
            if (dispatcherOperation.Status != DispatcherOperationStatus.Completed)
            {
                dispatcherOperation.Abort();
            }
        }

        private static object ExitFrameOperation(object obj)
        {
            ((DispatcherFrame)obj).Continue = false;
            return null;
        }

        private delegate object InvokeMethodDelegate(object o, object[] parameters);

        public static bool EnsureAccess(MethodBase method)
        {
            return EnsureAccess((Dispatcher)null, method, null);
        }

        public static bool EnsureAccess(MethodBase method, params object[] parameters)
        {
            return EnsureAccess(null, method, null, parameters);
        }

        public static bool EnsureAccess(MethodBase method, object o)
        {
            return EnsureAccess((Dispatcher)null, method, o);
        }

        public static bool EnsureAccess(MethodBase method, object o, params object[] parameters)
        {
            return EnsureAccess(null, method, o, parameters);
        }

        public static bool EnsureAccess(Dispatcher dispatcher, MethodBase method, object o, params object[] parameters)
        {
            if (dispatcher == null)
            {
                var dispatcherObj = o as DispatcherObject;
                if (o != null)
                {
                    dispatcher = dispatcherObj.Dispatcher;
                }
                else if (Application.Current != null)
                {
                    dispatcher = Application.Current.Dispatcher;
                }
                else
                {
                    throw new ArgumentException("Couldn't find an available dispatcher", nameof(dispatcher));
                }
            }

            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            bool hasAccess = dispatcher.CheckAccess();

            if (!hasAccess)
            {
                dispatcher.Invoke(
                    DispatcherPriority.Normal,
                    new InvokeMethodDelegate(method.Invoke),
                    o,
                    new object[] { parameters });
            }

            return hasAccess;
        }

        public static bool EnsureAccess(this DispatcherObject o, MethodBase method, params object[] parameters)
        {
            if (o is null)
            {
                throw new ArgumentNullException(nameof(o));
            }

            return EnsureAccess(o.Dispatcher, method, o, parameters);
        }

        public static void EnsureAccess(this Action action)
        {
            action.EnsureAccess(DispatcherPriority.Normal);
        }

        public static void EnsureAccess(this Action action, DispatcherPriority priority)
        {
            action.EnsureAccess(Application.Current.Dispatcher, priority);
        }

        public static void EnsureAccess(this Action action, Dispatcher dispatcher, DispatcherPriority priority)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (dispatcher is null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            if (!dispatcher.CheckAccess())
            {
                dispatcher.Invoke(action, priority);
            }
            else
            {
                action();
            }
        }

        public static DependencyObject FindLogicalAncestor(this DependencyObject startElement, Predicate<DependencyObject> condition)
        {
            if (condition is null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            DependencyObject o = startElement;
            while ((o != null) && !condition(o))
            {
                o = LogicalTreeHelper.GetParent(o);
            }

            return o;
        }

        public static T FindLogicalAncestorByType<T>(this DependencyObject startElement)
            where T : DependencyObject
        {
            return (T)FindLogicalAncestor(startElement, o => o is T);
        }

        public static DependencyObject FindLogicalRoot(this DependencyObject startElement)
        {
            DependencyObject o = null;
            while (startElement != null)
            {
                o = startElement;
                startElement = LogicalTreeHelper.GetParent(startElement);
            }

            return o;
        }

        public static DependencyObject FindVisualAncestor(this DependencyObject startElement, Predicate<DependencyObject> condition)
        {
            if (condition is null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            DependencyObject o = startElement;
            while ((o != null) && !condition(o))
            {
                o = VisualTreeHelper.GetParent(o);
            }

            return o;
        }

        public static T FindVisualAncestorByType<T>(this DependencyObject startElement)
            where T : DependencyObject
        {
            return (T)FindVisualAncestor(startElement, o => o is T);
        }

        public static DependencyObject FindVisualDescendant(this DependencyObject startElement, Predicate<DependencyObject> condition)
        {
            if (condition is null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            if (startElement != null)
            {
                if (condition(startElement))
                {
                    return startElement;
                }

                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(startElement); ++i)
                {
                    DependencyObject o = FindVisualDescendant(VisualTreeHelper.GetChild(startElement, i), condition);
                    if (o != null)
                    {
                        return o;
                    }
                }
            }

            return null;
        }

        public static T FindVisualDescendantByType<T>(this DependencyObject startElement)
            where T : DependencyObject
        {
            return (T)FindVisualDescendant(startElement, o => o is T);
        }

        public static DependencyObject FindVisualRoot(this DependencyObject startElement)
        {
            return FindVisualAncestor(startElement, o => VisualTreeHelper.GetParent(o) == null);
        }

        public static IEnumerable<Visual> GetVisualChildren(this Visual parent)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; ++i)
            {
                yield return (Visual)VisualTreeHelper.GetChild(parent, i);
            }
        }

        public static IEnumerable<DependencyObject> GetItemContainers(this ItemsControl itemsControl)
        {
            if (itemsControl is null)
            {
                yield break;
            }

            for (int i = 0; i < itemsControl.Items.Count; ++i)
            {
                yield return itemsControl.ItemContainerGenerator.ContainerFromIndex(i);
            }
        }

        public static bool IsInDesignMode()
        {
            return (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
        }

        public static bool IsAutomaticBrowserLaunchEnabled { get; set; }

        public static void LaunchBrowser(Uri uri)
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (!uri.IsAbsoluteUri)
            {
                return;
            }

            if (launchBrowserRequests >= MaxBrowserRequests)
            {
                return;
            }

            Interlocked.Increment(ref launchBrowserRequests);
            ThreadPool.QueueUserWorkItem(LaunchBrowserCallback, uri);
        }

        private static void LaunchBrowserCallback(object state)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = ((Uri)state).AbsoluteUri,
                };

                Process.Start(startInfo);
            }
            finally
            {
                Interlocked.Decrement(ref launchBrowserRequests);
            }
        }

        private static void OnHyperlinkClick(object sender, RoutedEventArgs e)
        {
            if (IsAutomaticBrowserLaunchEnabled)
            {
                Uri uri = ((Hyperlink)e.Source).NavigateUri;
                if (uri != null)
                {
                    LaunchBrowser(uri);
                }
            }
        }
    }
}

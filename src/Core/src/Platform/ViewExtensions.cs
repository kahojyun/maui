using System;
using System.Numerics;
using Microsoft.Maui.Graphics;
using System.Threading.Tasks;
using Microsoft.Maui.Media;
using System.IO;

#if (NETSTANDARD || !PLATFORM) || (NET6_0_OR_GREATER && !IOS && !ANDROID)
using IPlatformViewHandler = Microsoft.Maui.IViewHandler;
#endif
#if IOS || MACCATALYST
using PlatformView = UIKit.UIView;
using ParentView = UIKit.UIView;
#elif ANDROID
using PlatformView = Android.Views.View;
using ParentView = Android.Views.IViewParent;
#elif WINDOWS
using PlatformView = Microsoft.UI.Xaml.FrameworkElement;
using ParentView = Microsoft.UI.Xaml.DependencyObject;
#elif TIZEN
using PlatformView = Tizen.NUI.BaseComponents.View;
using ParentView = Tizen.NUI.BaseComponents.View;
#else
using PlatformView = System.Object;
using ParentView = System.Object;
#endif

namespace Microsoft.Maui.Platform
{
	/// <include file="../../docs/Microsoft.Maui/ViewExtensions.xml" path="Type[@FullName='Microsoft.Maui.ViewExtensions']/Docs/*" />
	public static partial class ViewExtensions
	{
		internal static Vector3 ExtractPosition(this Matrix4x4 matrix) => matrix.Translation;

		internal static Vector3 ExtractScale(this Matrix4x4 matrix) => new Vector3(matrix.M11, matrix.M22, matrix.M33);

		internal static double ExtractAngleInRadians(this Matrix4x4 matrix) => Math.Atan2(matrix.M21, matrix.M11);

		internal static double ExtractAngleInDegrees(this Matrix4x4 matrix) => ExtractAngleInRadians(matrix) * 180 / Math.PI;


		public static IPlatformViewHandler ToHandler(this IView view, IMauiContext context) =>
			(IPlatformViewHandler)ElementExtensions.ToHandler(view, context);

		internal static T? GetParentOfType<T>(this ParentView? view)
#if ANDROID
			where T : class, ParentView
#elif PLATFORM
			where T : ParentView
#else
			where T : class
#endif
		{
			if (view is T t)
				return t;

			while (view != null)
			{
				T? parent = view?.GetParent() as T;
				if (parent != null)
					return parent;

				view = view?.GetParent() as ParentView;
			}

			return default;
		}

		internal static ParentView? FindParent(this ParentView? view, Func<ParentView?, bool> searchExpression)
		{
			if (searchExpression(view))
				return view;

			while (view != null)
			{
				var parent = view?.GetParent() as ParentView;
				if (searchExpression(parent))
					return parent;

				view = view?.GetParent() as ParentView;
			}

			return default;
		}

#if WINDOWS || ANDROID
		internal static T? GetParentOfType<T>(this PlatformView view)
#if ANDROID
			where T : class, ParentView
#elif PLATFORM
			where T : ParentView
#else
			where T : class
#endif
		{
			if (view is T t)
				return t;

			return view.GetParent()?.GetParentOfType<T>();
		}
#endif

		internal static IDisposable OnUnloaded(this IElement element, Action action)
		{
#if PLATFORM
			if (element.Handler is IPlatformViewHandler platformViewHandler &&
				platformViewHandler.PlatformView != null)
			{
				return platformViewHandler.PlatformView.OnUnloaded(action);
			}

			throw new InvalidOperationException("Handler is not set on element");
#else
			throw new NotImplementedException();
#endif
		}

		internal static IDisposable OnLoaded(this IElement element, Action action)
		{
#if PLATFORM
			if (element.Handler is IPlatformViewHandler platformViewHandler &&
				platformViewHandler.PlatformView != null)
			{
				return platformViewHandler.PlatformView.OnLoaded(action);
			}

			throw new InvalidOperationException("Handler is not set on element");
#else
			throw new NotImplementedException();
#endif
		}

#if PLATFORM
		internal static Task OnUnloadedAsync(this PlatformView platformView, TimeSpan? timeOut = null)
		{
			timeOut = timeOut ?? TimeSpan.FromSeconds(2);
			TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
			platformView.OnUnloaded(() => taskCompletionSource.SetResult(true));
			return taskCompletionSource.Task.WaitAsync(timeOut.Value);
		}

		internal static Task OnLoadedAsync(this PlatformView platformView, TimeSpan? timeOut = null)
		{
			timeOut = timeOut ?? TimeSpan.FromSeconds(2);
			TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
			platformView.OnLoaded(() => taskCompletionSource.SetResult(true));
			return taskCompletionSource.Task.WaitAsync(timeOut.Value);
		}
#endif
		internal static bool IsLoadedOnPlatform(this IElement element)
		{
			if (element.Handler is not IPlatformViewHandler pvh)
				return false;

#if PLATFORM
			return pvh.PlatformView?.IsLoaded() == true;
#else
			return true;
#endif

		}
	}
}

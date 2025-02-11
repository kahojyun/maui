using Microsoft.Maui.Controls.CustomAttributes;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Graphics;

#if UITEST
using Microsoft.Maui.Controls.Compatibility.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Microsoft.Maui.Controls.ControlGallery.Issues
{
#if UITEST
	[Category(UITestCategories.InputTransparent)]
	[Category(UITestCategories.Gestures)]
	[Category(UITestCategories.Bugzilla)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 25943,
		"[Android] TapGestureRecognizer does not work with a nested StackLayout", PlatformAffected.Android)]
	public class Bugzilla25943 : TestContentPage
	{
		Label _result;
		int _taps;
		const string InnerLayout = "innerlayout";
		const string OuterLayout = "outerlayout";
		const string Success = "Success";

		protected override void Init()
		{
			StackLayout layout = GetNestedStackLayout();

			var tapGestureRecognizer = new TapGestureRecognizer();
			tapGestureRecognizer.Tapped += (sender, e) =>
			{
				_taps = _taps + 1;
				if (_taps == 2)
				{
					_result.Text = Success;
				}
			};
			layout.GestureRecognizers.Add(tapGestureRecognizer);

			Content = layout;
		}

		public StackLayout GetNestedStackLayout()
		{
			_result = new Label();

			var innerLayout = new StackLayout
			{
				AutomationId = InnerLayout,
				HeightRequest = 100,
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.Fill,
				BackgroundColor = Colors.AntiqueWhite,
				Children =
				{
					new Label
					{
						Text = "inner label",
						FontSize = 20,
						HorizontalOptions = LayoutOptions.Center,
						VerticalOptions = LayoutOptions.CenterAndExpand
					}
				}
			};

			var outerLayout = new StackLayout
			{
				AutomationId = OuterLayout,
				Orientation = StackOrientation.Vertical,
				BackgroundColor = Colors.Brown,
				Children =
				{
					_result,
					innerLayout,
					new Label
					{
						Text = "outer label",
						FontSize = 20,
						HorizontalOptions = LayoutOptions.Center,
					}
				}
			};

			return outerLayout;
		}


#if UITEST
		[Test]
		public void VerifyNestedStacklayoutTapsBubble()
		{
			RunningApp.WaitForElement(q => q.Marked(InnerLayout));
			RunningApp.Tap(InnerLayout);

			RunningApp.WaitForElement(q => q.Marked(OuterLayout));
			RunningApp.Tap(OuterLayout);

			RunningApp.WaitForElement(Success);
		}
#endif

	}
}
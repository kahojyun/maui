﻿using Microsoft.Maui.Controls.CustomAttributes;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Graphics;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Microsoft.Maui.Controls.Compatibility.UITests;
#endif

namespace Microsoft.Maui.Controls.ControlGallery.Issues
{
#if UITEST
	[Category(UITestCategories.Gestures)]
	[NUnit.Framework.Category(Compatibility.UITests.UITestCategories.UwpIgnore)]
	[NUnit.Framework.Category(Compatibility.UITests.UITestCategories.Bugzilla)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 59863, "TapGestureRecognizer extremely finicky", PlatformAffected.Android)]
	public class Bugzilla59863_0 : TestContentPage
	{
		int _singleTaps;
		const string SingleTapBoxId = "singleTapView";

		const string Singles = "singles(s)";

		protected override void Init()
		{
			var instructions = new Label
			{
				Text = "Tap the box below several times quickly. "
				+ "The number displayed below should match the number of times you tap the box."
			};

			var singleTapCounter = new Label { Text = $"{_singleTaps} {Singles}" };

			var singleTapBox = new BoxView
			{
				WidthRequest = 100,
				HeightRequest = 100,
				BackgroundColor = Colors.Bisque,
				AutomationId = SingleTapBoxId
			};

			var singleTap = new TapGestureRecognizer
			{
				Command = new Command(() =>
				{
					_singleTaps = _singleTaps + 1;
					singleTapCounter.Text = $"{_singleTaps} {Singles} on {SingleTapBoxId}";
				})
			};

			singleTapBox.GestureRecognizers.Add(singleTap);

			Content = new StackLayout
			{
				Margin = 40,
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill,
				Children = { instructions, singleTapBox, singleTapCounter }
			};
		}

#if UITEST
		[Test]
		public void TapsCountShouldMatch()
		{
			// Gonna add this test because we'd want to know if it _did_ start failing
			// But it doesn't really help much with this issue; UI test can't tap fast enough to demonstrate the 
			// problem we're trying to solve

			int tapsToTest = 5;

			RunningApp.WaitForElement(SingleTapBoxId);

			for (int n = 0; n < tapsToTest; n++)
			{
				RunningApp.Tap(SingleTapBoxId);
			}

			RunningApp.WaitForElement($"{tapsToTest} {Singles} on {SingleTapBoxId}");
		}

		[Test]
		public void DoubleTapWithOnlySingleTapRecognizerShouldRegisterTwoTaps()
		{
			RunningApp.WaitForElement(SingleTapBoxId);
			RunningApp.DoubleTap(SingleTapBoxId);

			RunningApp.WaitForElement($"2 {Singles} on {SingleTapBoxId}");
		}
#endif
	}
}
//
// ReplOptions.cs
//
// Author:
//       Matt Ward <matt.ward@microsoft.com>
//
// Copyright (c) 2021 Microsoft Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using Mono.Options;

namespace Microsoft.VisualStudio.AutoTest.Repl
{
	class ReplOptions
	{
		ReplOptions ()
		{
		}

		OptionSet GetOptionSet ()
		{
			return new OptionSet {
				{ "h|?|help", "Show help", h => Help = true },
				{ "a|attach", "Attach to Visual Studio for Mac", a => Attach = true },
				{ "f|file=", "Visual Studio for Mac path", f => FileName = f },
			};
		}

		public bool Help { get; private set; }
		public bool Attach { get; private set; }
		public string FileName { get; private set; }
		public List<string> RemainingArgs { get; private set; }
		public Exception Error { get; private set; }

		public bool HasError {
			get { return Error != null; }
		}

		public static ReplOptions Parse (string[] args)
		{
			var options = new ReplOptions ();

			if (args.Length > 0) {

				OptionSet optionsSet = options.GetOptionSet ();

				try {
					options.RemainingArgs = optionsSet.Parse (args);
				} catch (OptionException ex) {
					options.Error = ex;
				}
			} else {
				options.Help = true;
			}

			if (string.IsNullOrEmpty(options.FileName)) {
				options.Error = new ApplicationException ("--file argument is required");
			}

			return options;
		}

		public void ShowError()
		{
			if (Error == null)
				return;

			Console.WriteLine ("ERROR: {0}", Error);
			Console.WriteLine ("Pass --help for usage information.");
		}

		public void ShowHelp ()
		{
			Console.WriteLine ("Options:");

			OptionSet optionsSet = GetOptionSet ();
			optionsSet.WriteOptionDescriptions (Console.Out);
		}
	}
}

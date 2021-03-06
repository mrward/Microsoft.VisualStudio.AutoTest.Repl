//
// Program.cs
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
using System.IO;
using System.Reflection;
using Mono;
using Mono.CSharp;
using MonoDevelop.Components.AutoTest;

namespace Microsoft.VisualStudio.AutoTest.Repl
{
	class MainClass
	{
		static Evaluator evaluator;
		static ReplOptions options;

		public static int Main (string[] args)
		{
			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

			options = ReplOptions.Parse (args);
			if (options.Help) {
				options.ShowHelp ();
				return 0;
			}

			if (options.HasError) {
				options.ShowError ();
				return -1;
			}

			if (options.Attach) {
				return Attach ();
			} else {
				return Start (options.FileName);
			}
		}

		static int Start (string fileName)
		{
			string input = string.Format (
				"var app = new MonoDevelop.Components.AutoTest.AutoTestClientSession(); app.StartApplication(\"{0}\");",
				fileName);

			return StartRepl (input);
		}

		static int Attach ()
		{
			string input = "var app = new MonoDevelop.Components.AutoTest.AutoTestClientSession(); app.AttachApplication();";
			return StartRepl (input);
		}

		static int StartRepl (string initialInput = null)
		{
			var settings = new CompilerSettings () {
				Unsafe = true
			};

			var printer = new ConsoleReportPrinter ();

			evaluator = new Evaluator (new CompilerContext (settings, printer));

			evaluator.InteractiveBaseClass = typeof (InteractiveBase);
			evaluator.DescribeTypeExpressions = true;
			evaluator.WaitOnTask = true;

			var types = new Type[] {
				typeof (AutoTestClientSession),
				typeof (AppResult),
				typeof (AppQuery)
			};
			evaluator.ImportTypes(true, types);

			if (initialInput != null) {
				Evaluate (initialInput);
			}

			var shell = new CSharpShell (evaluator);

			return shell.Run (new string[0]);
		}

		static Assembly CurrentDomain_AssemblyResolve (object sender, ResolveEventArgs args)
		{
			var name = new AssemblyName (args.Name);

			string directory = Path.GetDirectoryName (options.FileName);
			string fullPath = Path.Combine (directory, name.Name + ".dll");
			if (File.Exists (fullPath)) {
				return Assembly.LoadFrom (fullPath);
			}
			return null;
		}

		static void Evaluate (string input)
		{
			try {
				evaluator.Evaluate (input, out object result, out bool result_set);
			} catch (Exception ex) {
				Console.WriteLine (ex);
			}
		}
	}
}

using Microsoft.Extensions.DependencyInjection;
using OpenTracing;
using System.Runtime.CompilerServices;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using Xunit;

namespace Helpers.Jaeger.Tests
{
	public sealed class WorkflowCoreTests : IClassFixture<Fixtures.TracerFixture>, IDisposable
	{
		private readonly ITracer _tracer;
		private readonly IServiceProvider _provider;
		private readonly IWorkflowHost _workflowHost;

		public WorkflowCoreTests(Fixtures.TracerFixture fixture)
		{
			_provider = new ServiceCollection()
				.AddLogging()
				.AddWorkflow()
				.AddOpenTracing()
				.AddSingleton(fixture.Tracer)
				.AddTransient<TestStep>()
				.BuildServiceProvider();

			_tracer = _provider.GetRequiredService<ITracer>();
			_workflowHost = _provider.GetRequiredService<IWorkflowHost>();

			_workflowHost.RegisterWorkflow<TestWorkflow, Data>();

			_workflowHost.Start();
		}

		[Fact]
		public async Task TracesAcrossSpans()
		{
			var data = new Data();

			using var scope = _tracer.StartSpan();

			Assert.NotNull(_tracer.ScopeManager.Active);

			Assert.Equal(0, data.Count);

			await _workflowHost.StartWorkflow(nameof(TestWorkflow), data);

			await Task.Delay(millisecondsDelay: 100);

			Assert.NotEqual(0, data.Count);
		}

		#region IDisposable implementation
		public void Dispose()
		{
			_workflowHost.Stop();
			(_provider as IDisposable)?.Dispose();
		}
		#endregion IDisposable implementation
	}

	public static class TracingExtensionMethods
	{
		private readonly static IDictionary<string, string> _textMap = new Dictionary<string, string>(1)
		{
			["message"] = "hello world",
		};

		public static IScope StartParentSpan(
			this ITracer tracer,
			[CallerMemberName] string? callerMethodName = default,
			[CallerFilePath] string? callerFilePath = default)
		{
			var operationName = GetOperationName(callerMethodName!, callerFilePath!);

			var scope = tracer
				.BuildSpan(operationName)
				.StartActive();

			tracer.Inject(
				scope.Span.Context,
				format: OpenTracing.Propagation.BuiltinFormats.TextMap,
				carrier: new OpenTracing.Propagation.TextMapInjectAdapter(_textMap));

			return scope;
		}

		public static IScope StartSpan(
			this ITracer tracer,
			[CallerMemberName] string? callerMethodName = default,
			[CallerFilePath] string? callerFilePath = default)
		{
			var operationName = GetOperationName(callerMethodName!, callerFilePath!);

			var context = tracer.Extract(
				format: OpenTracing.Propagation.BuiltinFormats.TextMap,
				carrier: new OpenTracing.Propagation.TextMapExtractAdapter(_textMap));

			if (context is null)
			{
				return tracer.StartParentSpan(callerMethodName, callerFilePath);
			}

			return tracer.BuildSpan(operationName)
				.AsChildOf(context)
				.StartActive();
		}

		private static string GetOperationName(string callerMethodName, string callerFilePath)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(callerMethodName);
			ArgumentException.ThrowIfNullOrWhiteSpace(callerFilePath);

			var fileName = System.IO.Path.GetFileNameWithoutExtension(callerFilePath);

			return $"{fileName}=>{callerMethodName}";
		}
	}

	public class TestStep : IStepBody
	{
		private readonly ITracer _tracer;

		public TestStep(ITracer tracer)
		{
			_tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
		}

		public int? Count { get; set; }

		public Task<ExecutionResult> RunAsync(IStepExecutionContext context)
		{
			using var scope = _tracer.StartSpan();

			Count++;

			scope.Span.Log(new Dictionary<string, object?>(1)
			{
				[nameof(Count)] = Count,
			});

			return Task.FromResult(ExecutionResult.Next());
		}
	}

	public class TestWorkflow : IWorkflow<Data>
	{
		public string Id => nameof(TestWorkflow);
		public int Version => 1;

		public void Build(IWorkflowBuilder<Data> builder)
		{
			builder
				.StartWith<TestStep>()
					.Input(step => step.Count, data => data.Count)
					.Output(data => data.Count, step => step.Count)
				.Then<TestStep>()
					.Input(step => step.Count, data => data.Count)
					.Output(data => data.Count, step => step.Count)
				.EndWorkflow();
		}
	}

	public class Data
	{
		public int Count { get; set; } = 0;
	}
}

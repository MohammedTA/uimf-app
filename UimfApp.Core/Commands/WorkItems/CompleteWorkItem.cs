﻿namespace UimfApp.Core.Commands.WorkItems
{
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	using MediatR;
	using UimfApp.Core.DataAccess;
	using UimfApp.Core.Security.WorkItem;
	using UimfApp.Infrastructure;
	using UimfApp.Infrastructure.Forms;
	using UimfApp.Infrastructure.Security;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Core;
	using UiMetadataFramework.Core.Binding;

	[MyForm(Id = "complete-work-item", Label = "Complete work item")]
	[Secure(typeof(WorkItemAction), nameof(WorkItemAction.ManageWorkItem))]
	public class CompleteWorkItem : MyAsyncForm<CompleteWorkItem.Request, CompleteWorkItem.Response>
	{
		private readonly CoreDbContext context;

		public CompleteWorkItem(CoreDbContext context)
		{
			this.context = context;
		}

		public static FormLink Button(int workItemId, string label = null)
		{
			return new FormLink
			{
				Label = label ?? "Complete",
				Form = typeof(CompleteWorkItem).GetFormId(),
				InputFieldValues = new Dictionary<string, object>
				{
					{ nameof(Request.Id), workItemId }
				},
				Action = FormLinkActions.Run
			};
		}

		public override async Task<Response> Handle(Request request, CancellationToken cancellationToken)
		{
			var item = await this.context.WorkItems.SingleOrExceptionAsync(t => t.Id == request.Id);

			item.Complete();
			await this.context.SaveChangesAsync(cancellationToken);

			return new Response();
		}

		public class Response : FormResponse<MyFormResponseMetadata>
		{
		}

		public class Request : IRequest<Response>, ISecureHandlerRequest
		{
			[InputField(Hidden = true)]
			public int Id { get; set; }

			[NotField]
			public int ContextId => this.Id;
		}
	}
}
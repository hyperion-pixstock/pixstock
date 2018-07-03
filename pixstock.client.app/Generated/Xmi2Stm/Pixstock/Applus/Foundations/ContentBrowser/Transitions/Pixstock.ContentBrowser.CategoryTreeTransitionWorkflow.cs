using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Hyperion.Pf.Workflow;
using Appccelerate.StateMachine;
using Appccelerate.StateMachine.Infrastructure;
using Appccelerate.StateMachine.Machine;
using Appccelerate.StateMachine.Persistence;
using Appccelerate.StateMachine.Reports;
using Hyperion.Pf.Workflow.StateMachine;

using Pixstock.Core;
namespace Pixstock.Applus.Foundations.ContentBrowser.Transitions {
public partial class CategoryTreeTransitionWorkflow : FrameStateMachine<States, Events>, IAsyncPassiveStateMachine {
	public static string Name = "Pixstock.ContentBrowser.CategoryTreeTransitionWorkflow";
public void Setup() {
DefineHierarchyOn(States.ROOT)
.WithHistoryType(HistoryType.None)
.WithInitialSubState(States.Dashboard)
;
DefineHierarchyOn(States.Dashboard)
.WithHistoryType(HistoryType.None)
.WithInitialSubState(States.DashboardBase)
.WithSubState(States.CategoryList)
;
DefineHierarchyOn(States.CategoryList)
.WithHistoryType(HistoryType.None)
.WithInitialSubState(States.CategoryListBase)
.WithSubState(States.Preview)
;
In(States.INIT)
.On(Events.TRNS_TOPSCREEN)
.Goto(States.Dashboard);
In(States.ROOT)
.On(Events.TRNS_EXIT)
.Goto(States.INIT);
In(States.ROOT)
.On(Events.TRNS_DEBUG_BACK)
.Goto(States.ROOT);
In(States.DashboardBase)
.On(Events.TRNS_ThumbnailListPage)
.Goto(States.CategoryList);
In(States.CategoryListBase)
.On(Events.TRNS_PreviewPage)
.Goto(States.Preview);
In(States.CategoryListBase)
.On(Events.TRNS_BACK)
.Goto(States.Dashboard);
In(States.Preview)
.On(Events.TRNS_BACK)
.Goto(States.CategoryListBase);
In(States.ROOT)
.On(Events.ACT_DEBUGCOMMAND)
.Execute<object>(ACT_DEBUGCOMMAND);
In(States.ROOT)
.On(Events.ACT_CATEGORYTREE_UPDATE)
.Execute<object>(ACT_CATEGORYTREE_UPDATE);
In(States.ROOT)
.On(Events.ACT_THUMBNAILLIST_UPDATE)
.Execute<object>(ACT_THUMBNAILLIST_UPDATE);
In(States.ROOT)
.On(Events.ACT_PREVIEW_UPDATE)
.Execute<object>(ACT_PREVIEW_UPDATE);
In(States.Dashboard)
.ExecuteOnEntry(__FTC_Event_Dashboard_Entry);
In(States.Dashboard)
.ExecuteOnExit(__FTC_Event_Dashboard_Exit);
In(States.DashboardBase)
.ExecuteOnEntry(HomePageBase_Entry);
In(States.DashboardBase)
.ExecuteOnExit(HomePageBase_Exit);
In(States.CategoryList)
.ExecuteOnEntry(ThumbnailListPage_Entry);
In(States.CategoryList)
.ExecuteOnExit(ThumbnailListPage_Exit);
In(States.CategoryListBase)
.On(Events.CategorySelectBtnClick)
.Execute<object>(CategorySelectBtnClick);
In(States.CategoryListBase)
.On(Events.ACT_ContinueCategoryList)
.Execute<object>(ACT_ContinueCategoryList);
In(States.CategoryListBase)
.On(Events.ACT_UpperCategoryList)
.Execute<object>(ACT_UpperCategoryList);
In(States.Preview)
.ExecuteOnEntry(__FTC_Event_Preview_Entry);
In(States.Preview)
.ExecuteOnExit(__FTC_Event_Preview_Exit);
In(States.Preview)
.On(Events.ACT_DISPLAY_PREVIEWCURRENTLIST)
.Execute<object>(ACT_DISPLAY_PREVIEWCURRENTLIST);
In(States.Preview)
.On(Events.RESPONSE_GETCONTENT)
.Execute<object>(RESPONSE_GETCONTENT);
	Initialize(States.INIT);
}
public virtual async Task ACT_DEBUGCOMMAND(object param) {
	Events.ACT_DEBUGCOMMAND.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_DEBUGCOMMAND(param);
	Events.ACT_DEBUGCOMMAND.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task ACT_CATEGORYTREE_UPDATE(object param) {
	Events.ACT_CATEGORYTREE_UPDATE.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_CATEGORYTREE_UPDATE(param);
	Events.ACT_CATEGORYTREE_UPDATE.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task ACT_THUMBNAILLIST_UPDATE(object param) {
	Events.ACT_THUMBNAILLIST_UPDATE.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_THUMBNAILLIST_UPDATE(param);
	Events.ACT_THUMBNAILLIST_UPDATE.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task ACT_PREVIEW_UPDATE(object param) {
	Events.ACT_PREVIEW_UPDATE.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_PREVIEW_UPDATE(param);
	Events.ACT_PREVIEW_UPDATE.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task __FTC_Event_Dashboard_Entry() {
ICollection<int> ribbonMenuEventId = new List<int>{  };
	ShowFrame("Dashboard",ribbonMenuEventId);
}
public virtual async Task __FTC_Event_Dashboard_Exit() {
ICollection<int> ribbonMenuEventId = new List<int>{  };
	HideFrame("Dashboard", ribbonMenuEventId);
}
public virtual async Task HomePageBase_Entry() {
	await OnHomePageBase_Entry();
}
public virtual async Task HomePageBase_Exit() {
	await OnHomePageBase_Exit();
}
public virtual async Task ThumbnailListPage_Entry() {
	await OnThumbnailListPage_Entry();
ICollection<int> ribbonMenuEventId = new List<int>{  };
	ShowFrame("CategoryList",ribbonMenuEventId);
}
public virtual async Task ThumbnailListPage_Exit() {
	await OnThumbnailListPage_Exit();
ICollection<int> ribbonMenuEventId = new List<int>{  };
	HideFrame("CategoryList", ribbonMenuEventId);
}
public virtual async Task CategorySelectBtnClick(object param) {
	Events.CategorySelectBtnClick.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnCategorySelectBtnClick(param);
	Events.CategorySelectBtnClick.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task ACT_ContinueCategoryList(object param) {
	Events.ACT_ContinueCategoryList.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_ContinueCategoryList(param);
	Events.ACT_ContinueCategoryList.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task ACT_UpperCategoryList(object param) {
	Events.ACT_UpperCategoryList.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_UpperCategoryList(param);
	Events.ACT_UpperCategoryList.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task __FTC_Event_Preview_Entry() {
ICollection<int> ribbonMenuEventId = new List<int>{  };
	ShowFrame("Preview",ribbonMenuEventId);
}
public virtual async Task __FTC_Event_Preview_Exit() {
ICollection<int> ribbonMenuEventId = new List<int>{  };
	HideFrame("Preview", ribbonMenuEventId);
}
public virtual async Task ACT_DISPLAY_PREVIEWCURRENTLIST(object param) {
	Events.ACT_DISPLAY_PREVIEWCURRENTLIST.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_DISPLAY_PREVIEWCURRENTLIST(param);
	Events.ACT_DISPLAY_PREVIEWCURRENTLIST.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task RESPONSE_GETCONTENT(object param) {
	Events.RESPONSE_GETCONTENT.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnRESPONSE_GETCONTENT(param);
	Events.RESPONSE_GETCONTENT.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
}
}

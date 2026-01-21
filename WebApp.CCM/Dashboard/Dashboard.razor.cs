
using Radzen.Blazor.Markdown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static WebApp.CCM.Dashboard.Dashboard;

namespace WebApp.CCM.Dashboard
{
	public partial class Dashboard
	{
	
	// -------------------- ENUMS --------------------
	public enum IssueState
		{
			Open,
			Closed
		}

		// -------------------- MODELS --------------------
		public class Issue
		{
			public int Id { get; set; }
			public string Title { get; set; } = string.Empty;
			public string Url { get; set; } = "#";
			public IssueState State { get; set; }
			public DateTime CreatedAt { get; set; }
			public User User { get; set; } = new();
			public List<Label> Labels { get; set; } = new();
		}

		public class User
		{
			public string Login { get; set; } = string.Empty;
			public string AvatarUrl { get; set; } = string.Empty;
		}

		public class Label
		{
			public string Name { get; set; } = string.Empty;
			public string Color { get; set; } = "cccccc";
		}

		public class IssueByDate
		{
			public DateTime Week { get; set; }
			public int Count { get; set; }
		}

		public class GroupCount
		{
			public string Name { get; set; } = string.Empty;
			public int Count { get; set; }
		}

		public class LabelGroup
		{
			public string Label { get; set; } = string.Empty;
			public int Count { get; set; }
		}

		// -------------------- STATE --------------------
		bool fetchingData = false;
		string? error;

		int currentPage = 1;
		int totalPages = 5;

		List<Issue> issues = new();
		IEnumerable<Issue> openIssues = Enumerable.Empty<Issue>();
		IEnumerable<Issue> closedIssues = Enumerable.Empty<Issue>();
		IEnumerable<Issue> filteredIssues = Enumerable.Empty<Issue>();

		List<IssueByDate> openIssuesByDate = new();
		List<IssueByDate> closedIssuesByDate = new();

		List<GroupCount> openByGroups = new();
		List<LabelGroup> labelGroups = new();

		List<string> labelColors = new() { "#4CAF50", "#FF9800", "#2196F3", "#F44336" };

		double closeRatioPercentage;

		List<IssueState> issueStates = Enum.GetValues<IssueState>().ToList();
		IssueState selectedState = IssueState.Open;

		List<string> labels = new();
		List<string> selectedLabels = new();

		List<User> users = new();
		User? selectedUser;

		User? mostActiveMember;

		// -------------------- LIFECYCLE --------------------
		protected override void OnInitialized()
		{
			SeedData();
			CalculateDerivedData();
			FilterIssues(null);
		}

		// -------------------- DATA --------------------
		void SeedData()
		{
			users = new()
		{
			new User { Login = "alice", AvatarUrl = "https://i.pravatar.cc/150?img=1" },
			new User { Login = "bob", AvatarUrl = "https://i.pravatar.cc/150?img=2" },
			new User { Login = "charlie", AvatarUrl = "https://i.pravatar.cc/150?img=3" }
		};

			var labelBug = new Label { Name = "bug", Color = "f44336" };
			var labelFeature = new Label { Name = "feature", Color = "4caf50" };
			var labelDocs = new Label { Name = "docs", Color = "2196f3" };

			issues = new List<Issue>
		{
			new Issue
			{
				Id = 1,
				Title = "Login page error",
				State = IssueState.Open,
				CreatedAt = DateTime.Today.AddDays(-6),
				User = users[0],
				Labels = new() { labelBug }
			},
			new Issue
			{
				Id = 2,
				Title = "Add dark mode",
				State = IssueState.Closed,
				CreatedAt = DateTime.Today.AddDays(-5),
				User = users[1],
				Labels = new() { labelFeature }
			},
			new Issue
			{
				Id = 3,
				Title = "Update documentation",
				State = IssueState.Open,
				CreatedAt = DateTime.Today.AddDays(-4),
				User = users[2],
				Labels = new() { labelDocs }
			},
			new Issue
			{
				Id = 4,
				Title = "Crash on submit",
				State = IssueState.Closed,
				CreatedAt = DateTime.Today.AddDays(-3),
				User = users[0],
				Labels = new() { labelBug, labelFeature }
			}
		};

			labels = issues.SelectMany(i => i.Labels).Select(l => l.Name).Distinct().ToList();
		}

		// -------------------- CALCULATIONS --------------------
		void CalculateDerivedData()
		{
			openIssues = issues.Where(i => i.State == IssueState.Open);
			closedIssues = issues.Where(i => i.State == IssueState.Closed);

			closeRatioPercentage = issues.Count == 0
				? 0
				: Math.Round((double)closedIssues.Count() / issues.Count * 100, 2);

			openIssuesByDate = openIssues
				.GroupBy(i => i.CreatedAt.Date)
				.Select(g => new IssueByDate { Week = g.Key, Count = g.Count() })
				.ToList();

			closedIssuesByDate = closedIssues
				.GroupBy(i => i.CreatedAt.Date)
				.Select(g => new IssueByDate { Week = g.Key, Count = g.Count() })
				.ToList();

			openByGroups = openIssues
				.GroupBy(i => i.User.Login)
				.Select(g => new GroupCount { Name = g.Key, Count = g.Count() })
				.ToList();

			labelGroups = issues
				.SelectMany(i => i.Labels)
				.GroupBy(l => l.Name)
				.Select(g => new LabelGroup { Label = g.Key, Count = g.Count() })
				.ToList();

			mostActiveMember = issues
				.GroupBy(i => i.User)
				.OrderByDescending(g => g.Count())
				.Select(g => g.Key)
				.FirstOrDefault();
		}

		// -------------------- FILTERING --------------------
		void FilterIssues(object? value)
		{
			filteredIssues = issues.Where(i => i.State == selectedState);

			if (selectedLabels.Any())
				filteredIssues = filteredIssues.Where(i =>
					i.Labels.Any(l => selectedLabels.Contains(l.Name)));

			if (selectedUser != null)
				filteredIssues = filteredIssues.Where(i => i.User.Login == selectedUser.Login);
		}

	}
}

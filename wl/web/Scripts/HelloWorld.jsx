var dateUtility = (function() {
	return {
		millisecondsToHoursAndMinutes: function(duration) {
			var hours = Math.floor(duration / 3600000);
		
			var minutes = ((duration % 3600000) / 60000).toFixed(0);
			return hours + ":" + (minutes < 10 ? '0' : '') + minutes;
		},
		formatDate: function(time) {
			var hh = time.getHours();
			var tt = "AM";
			if(hh > 12) {
				tt = "PM";
				hh -= 12;
			}
			var mm = time.getMinutes();
			var MM = time.getMonth();
			var dd = time.getDate();
			var yyyy = time.getFullYear();

			return hh.toString() + 
				":" +
				mm.toString() +
				" " +
				tt + 
				" " +
				MM.toString() + 
				"/" +
				dd.toString() + 
				"/" +
				yyyy.toString();
		},
	};
})();

var WorklogHeader = React.createClass({
	render: function() {
		return <thead><tr><th></th><th>Job ID</th><th>Start Time</th><th>End Time</th><th>Duration</th><th>Comment</th></tr></thead>;
	}
});

var WorklogFooter = React.createClass({
	getJobIdParts: function(jobid) {
		return {
			tag: jobid.substring(jobid.length - 1).split(':')[0].trim(),
			id: jobid.substring(0, jobid.length - 1).split(':')[1].trim()
		};
	},
	isInCategory: function(worklog, category) {
		if(!category.tag || !worklog.jobid || category.id)
			return false;
		return worklog.jobid.startsWith('[' + category.tag);
	},
	render : function() {
		var self = this;
		var categories = {
			"total" : { total : 0 },
			"features" : { tag: "axof", total: 0 },
			"defects" : { tag: "axod", total: 0 },
			"kanban" : { tag: "axot", total: 0 },
			"helpdesk" : { tag: "axoi", total: 0 },
			"empty" : { total: 0 },
		};
		this.props.worklogs.forEach(function(worklog) { 
			if(worklog.jobid && !categories[worklog.jobid]) {
				var jobIdParts = self.getJobIdParts(worklog.jobid);
				categories[worklog.jobid] = { total: 0, tag: jobIdParts.tag, id: jobIdParts.id };
			}
			if(worklog.durationticks) { 
				categories.total.total += worklog.durationticks;
				if(!worklog.jobid) 
					categories.empty.total += worklog.durationticks;
				else
					categories[worklog.jobid].total += worklog.durationticks;

				for(var category in categories) {
					if(worklog.jobid && self.isInCategory(worklog, categories[category])) {
						categories[category].total += worklog.durationticks;
					}
				}
			} 
		});

		var totals = [];
		for(var total in categories) {
			totals.push({
				name: total,
				value: categories[total]
			});
		}

		return <tfoot>
			{totals.map((total) =>
				<tr><td></td><td>{total.name}</td><td></td><td></td><td>{dateUtility.millisecondsToHoursAndMinutes(total.value.total)}</td></tr>)}
		</tfoot>;

	}
});

var WorklogTableBody = React.createClass({
	handleWorklogChange: function(event, index) {
		return this.props.onChange(event, index);
	},
	render: function () {
		return <tbody>
			{this.props.worklogs.map((worklog, index) =>
				<WorklogTableRow key={index} index={index} worklog={worklog} onChange={this.handleWorklogChange} />)}
		</tbody>;
	}
});

var WorklogTableRow = React.createClass({
	handleCheckChange: function(event) {
		this.props.onChange(event, this.props.index);
	},
	render: function () {
		return <tr>
			<td><input type="checkbox" onChange={this.handleCheckChange} checked={this.props.worklog.checked}/></td>
			<td>{this.props.worklog.jobid}</td>
			<td>{this.props.worklog.starttime}</td>
			<td>{this.props.worklog.endtime}</td>
			<td>{this.props.worklog.duration}</td>
			<td>{this.props.worklog.comment}</td>
		</tr>;
	}
});

var WorklogTable = React.createClass({
	getInitialState: function () {		
		return { "worklogs" : this.calculateStopTimes(this.props.model) };
	},
	handleWorklogChange: function(event, index) {
		var worklogs = this.state.worklogs;
		
		this.setChecked(worklogs, index, event);
		
		this.setState({
			worklogs: this.calculateStopTimes(worklogs)
		});
	},
	setChecked: function(worklogs, index, event) {
		var worklog = worklogs[index];
		worklog.checked = event.target.checked;
	},
	calculateStopTimes: function(worklogs) {
		worklogs
			.filter(function(worklog) { return !worklog.checked; })
			.forEach(function(worklog) { 
				worklog.duration = null;
				worklog.endtime = null; 
				worklog.durationticks = null;
			});
		var checkedLogs = worklogs.filter(function(worklog) { return worklog.checked; });

		for(var i = 0; i < checkedLogs.length; i++) {
			var worklog = checkedLogs[i];
			if(checkedLogs.length > i + 1) {
				var duration = new Date(checkedLogs[i+1].starttime) - new Date(worklog.starttime);
				worklog.duration = dateUtility.millisecondsToHoursAndMinutes(duration);
				worklog.endtime = dateUtility.formatDate(new Date(checkedLogs[i+1].starttime));
				worklog.durationticks = duration;
			}
			else {
				worklog.duration = null;
				worklog.endtime = null;
				worklog.durationticks = null;
			}
		}
		return worklogs;			
	},
	render: function() {
		return <table className="table table-striped table-condensed">
			<WorklogHeader />
			<WorklogTableBody worklogs={this.state.worklogs} onChange={this.handleWorklogChange} />
			<WorklogFooter worklogs={this.state.worklogs} />
		</table>;
	}
});

var model = [
	{
		"checked" : true,
		"starttime" : "8:33 AM 10/18/2016",
		"jobid" : "[axof: 9983]",
		"comment" : "checking for buyerzone errors"
	},
	{
		"checked" : true,
		"starttime" : "8:45 AM 10/18/2016",
		"jobid" : "[axof: 6048]",
		"comment" : "standup and post standup discussions"
	},
	{
		"checked" : true,
		"starttime" : "9:01 AM 10/18/2016",
		"jobid" : "[axof: 8332]",
		"comment" : "time entry"
	},
	{
		"checked" : true,
		"starttime" : "9:06 AM 10/18/2016",
		"jobid" : "[axof: 17137]",
		"comment" : null
	},
	{
		"checked" : true,
		"starttime" : "9:15 AM 10/18/2016",
		"jobid" : "[axof: 8307]",
		"comment" : "talking to sean about his task"
	},
	{
		"checked" : true,
		"starttime" : "9:22 AM 10/18/2016",
		"jobid" : "[axof: 17137]",
		"comment" : null
	},
	{
		"checked" : true,
		"starttime" : "9:33 AM 10/18/2016",
		"jobid" : "[axof: 9983]",
		"comment" : "clearing out toyota staging"
	},
	{
		"checked" : true,
		"starttime" : "9:47 AM 10/18/2016",
		"jobid" : "[axof: 17137]",
		"comment" : null
	},
	{
		"checked" : true,
		"starttime" : "11:42 AM 10/18/2016",
		"jobid" : "[axof: 16931]",
		"comment" : null
	},
	{
		"checked" : true,
		"starttime" : "12:55 PM 10/18/2016",
		"jobid" : null,
		"comment" : "lunch"
	},
	{
		"checked" : true,
		"starttime" : "1:46 PM 10/18/2016",
		"jobid" : "[axof: 16931]",
		"comment" : null
	},
	{
		"checked" : true,
		"starttime" : "2:00 PM 10/18/2016",
		//"starttime" : "5:07 PM 10/18/2016",
		"jobid" : "[axof: 8307]",
		"comment" : "talking to sean about his task"
	},
	{
		"checked" : true,
		"starttime" : "2:11 PM 10/18/2016",
		"jobid" : "[axof: 16931]",
		"comment" : null
	},
	{
		"checked" : true,
		"starttime" : "2:30 PM 10/18/2016",
		"jobid" : "[axof: 6049]",
		"comment" : "backlog grooming"
	},
	{
		"checked" : true,
		"starttime" : "4:31 PM 10/18/2016",
		"jobid" : "[axof: 16931]",
		"comment" : null
	},
	{
		"checked" : true,
		"starttime" : "5:07 PM 10/18/2016",
		"jobid" : null,
		"comment" : null
	},
];

ReactDOM.render(
  <WorklogTable model={model} />,
  document.getElementById('container')
);
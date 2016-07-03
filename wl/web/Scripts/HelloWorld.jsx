var Hello = React.createClass({
  render: function() {
    return <div>Hello {this.props.name}, {this.props.user}</div>;
  }
});

var WorklogHeader = React.createClass({
	render: function() {
		return <thead><tr></tr><tr><th>Job ID</th><th>Start Time</th><th>End Time</th><th>Duration</th><th>Title</th><th>Comment</th></tr></thead>;
	}
});

var WorklogTableBody = React.createClass({
	render: function () {
		return <tbody></tbody>;
	}
});

var WorklogTableRow = React.createClass({
	render: function () {.
		return <tr><td><input type="checkbox" checked="{this.props.checked}"/></td><td>{this.props.jobid}</td><td>{this.props.starttime}</td><td>{this.props.endtime}</td><td>{this.props.duration}</td><td>{this.props.title}</td><td>{this.props.comment}</td></tr>;
	}
});

var WorklogTable = React.createClass({
	render: function() {
		return <table>
			<WorklogHeader />
			<WorklogTableBody />
		</table>;
	}
});

ReactDOM.render(
  <WorklogTable />,
  document.getElementById('container')
);
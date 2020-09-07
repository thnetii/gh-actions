const core = require('@actions/core');

try {
  core.setOutput('value', core.getInput('input'));
} catch (error) {
  core.setFailed(error);
}

const core = require('@actions/core');
const process = require('process');
const path = require('path');
const fs = require('fs');
const yaml = require('js-yaml');

const github_action_path = process.env['GITHUB_ACTION_PATH'] ||
  path.normalize(path.join(__filename, '..', '..', '..'));
const github_workspace_path = process.env['GITHUB_WORKSPACE'] ||
  github_action_path;

try {
  process.chdir(github_workspace_path);

} catch (error) {
  core.setFailed(error);
}

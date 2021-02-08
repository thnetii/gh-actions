import { EOL } from 'os';
import * as core from '@actions/core';
import * as glob from '@actions/glob';
import * as exec from '@actions/exec';

const inputs = {
  command: core.getInput('command', { required: true }),
  project: core.getInput('project', { required: false }),
  configuration: core.getInput('configuration', { required: false }),
  framework: core.getInput('framework', { required: false }),
  runtime: core.getInput('runtime', { required: false }),
  nuget_source: core.getInput('nuget-source', { required: false }),
  nuget_packages_dir: core.getInput('nuget-packages-dir', { required: false }),
  nuget_configfile: core.getInput('nuget-configfile', { required: false }),
  dotnet_arguments: core.getInput('dotnet-arguments', { required: false })?.split(EOL).filter(i => i),
  msbuild_arguments: core.getInput('msbuild-arguments', { required: false })?.split(EOL).filter(i => i),
  verbosity: core.getInput('verbosity', { required: false }),
  binlogDir: core.getInput('binary-log-directory', { required: false })
};

console.log(inputs);

glob.create(inputs.project).then(globber => {
  const projGlobs = inputs.project.split(EOL);
  let projGlobMessage = 'Input project glob patterns:';
  if (projGlobs.length) {
    for (const projGlobPattern of projGlobs) {
      projGlobMessage += EOL + '  - ' + projGlobPattern;
    }
  } else {
    projGlobMessage += ' []';
  }
  core.info(projGlobMessage);

  globber.glob().then(projGlobResults => {
    let projGlobResultMessage = 'Resolved project glob paths:';
    if (projGlobResults.length) {
      for (const projGlobResultPath of projGlobResults) {
        projGlobResultMessage += EOL + '  - ' + projGlobResultPath;
      }
    } else {
      projGlobResultMessage += ' []';
    }
    core.info(projGlobResultMessage);
  });
});

import { EOL } from 'os';
import * as path from 'path';
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
  dotnet_arguments: core.getInput('dotnet-arguments', { required: false })?.split('\n').filter(i => i),
  msbuild_arguments: core.getInput('msbuild-arguments', { required: false })?.split('\n').filter(i => i),
  verbosity: core.getInput('verbosity', { required: false }),
  binlogDir: core.getInput('binary-log-directory', { required: false })
};

console.log(inputs);

let preProjectArguments = [
  inputs.command,
  ...inputs.dotnet_arguments
];
let postProjectArguments = [
  ...inputs.msbuild_arguments
];

if (inputs.configuration) {
  if (['build', 'pack', 'publish', 'test'].includes(inputs.command)) {
    preProjectArguments.push('--configuration', inputs.configuration);
  } else {
    postProjectArguments.push(`-property:Configuration=${inputs.configuration}`);
  }
}
if (inputs.framework) {
  if (['build', 'publish', 'test'].includes(inputs.command)) {
    preProjectArguments.push('--framework', inputs.framework);
  } else {
    postProjectArguments.push(`-property:TargetFramework=${inputs.framework}`);
  }
}
if (inputs.runtime) {
  if (['restore', 'build', 'pack', 'publish', 'test'].includes(inputs.command)) {
    preProjectArguments.push('--runtime', inputs.runtime);
  } else {
    postProjectArguments.push(`-property:RuntimeIdentifier=${inputs.runtime}`);
  }
}
if (inputs.nuget_source) {
  if (inputs.command == 'restore') {
    preProjectArguments.push('--source', inputs.nuget_source);
  } else {
    postProjectArguments.push(`-property:RestoreSources=${inputs.nuget_source}`);
  }
}
if (inputs.nuget_packages_dir) {
  if (inputs.command == 'restore') {
    preProjectArguments.push('--packages', inputs.nuget_packages_dir);
  } else {
    postProjectArguments.push(`-property:RestorePackagesPath=${inputs.nuget_packages_dir}`);
  }
}
if (inputs.nuget_configfile) {
  if (inputs.command == 'restore') {
    preProjectArguments.push('--configfile', inputs.nuget_configfile);
  } else {
    postProjectArguments.push(`-property:RestoreConfigFile=${inputs.nuget_configfile}`);
  }
}
if (inputs.verbosity) {
  preProjectArguments.push('--verbosity', inputs.verbosity);
}

glob.create(inputs.project).then(globber => {
  const projGlobs = inputs.project.split('\n');
  let projGlobMessage = 'Input project glob patterns:';
  if (projGlobs.length) {
    for (const projGlobPattern of projGlobs) {
      projGlobMessage += EOL + '  - ' + projGlobPattern;
    }
  } else {
    projGlobMessage += ' []';
  }
  core.info(projGlobMessage);

  globber.glob().then(async projGlobResults => {
    let projGlobResultMessage = 'Resolved project glob paths:';
    if (projGlobResults.length) {
      for (const projGlobResultPath of projGlobResults) {
        projGlobResultMessage += EOL + '  - ' + projGlobResultPath;
      }
    } else {
      projGlobResultMessage += ' []';
    }
    core.info(projGlobResultMessage);

    for (const projGlobResultPath of projGlobResults) {
      core.startGroup(projGlobResultPath);
      try {
        let invokeArguments = [];
        invokeArguments.push(...preProjectArguments);
        invokeArguments.push(projGlobResultPath);
        invokeArguments.push(...postProjectArguments);
        if (inputs.binlogDir) {
          const binlogFileName = `${path.basename(projGlobResultPath)}.${inputs.command}.binlog`;
          const binlogFilePath = path.join(inputs.binlogDir, binlogFileName);
          let binlogArg = `-bl:${path.normalize(binlogFilePath)}`;
          invokeArguments.push(binlogArg);
        }

        try {
          await exec.exec('dotnet', invokeArguments);
        } catch (e) {
          core.setFailed(e);
          return;
        }
      }
      finally { core.endGroup(); }
    }
  });
});

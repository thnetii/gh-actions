import * as fs from 'fs';
import * as path from 'path';
import * as core from '@actions/core';
import * as exec from '@actions/exec';
import * as glob from '@actions/glob';

const inputs = {
  source: core.getInput('source', { required: true }),
  api_key: core.getInput('api-key', { required: false }),
  skip_duplicate: (() => {
    const inputValue = core.getInput('skip-duplicate', { required: false });
    return inputValue ? Boolean(JSON.parse(inputValue)) : false;
  })(),
};

const asyncMain = async () => {
  const globber = await glob.create('**/*.nupkg');
  const globResult = await globber.glob();
  for (const nupkgPathString of globResult) {
    const execArgs = [
      'nuget', 'push'
    ];
    execArgs.push(nupkgPathString);

    const nupkgPathParsed = path.parse(nupkgPathString);
    const symbolsPathString = `${nupkgPathParsed.dir}${nupkgPathParsed.name}.snupkg`;
    try {
      fs.accessSync(symbolsPathString, fs.constants.R_OK);
      execArgs.push(symbolsPathString);
    } catch (err) {
      core.debug(`${symbolsPathString}: ${err}`);
    }

    execArgs.push('--source', inputs.source);

    if (inputs.api_key)
      execArgs.push('--api-key', inputs.api_key);

    if (inputs.skip_duplicate)
      execArgs.push('--skip-duplicate');

    const pushErrorCode = await exec.exec('dotnet', execArgs);
    if (pushErrorCode)
      core.setFailed(`Failed to push nupkg file: ${nupkgPathParsed.name}`);
  }
};
asyncMain();

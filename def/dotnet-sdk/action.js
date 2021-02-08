"use strict";
var _a, _b;
exports.__esModule = true;
var os_1 = require("os");
var core = require("@actions/core");
var glob = require("@actions/glob");
var inputs = {
    command: core.getInput('command', { required: true }),
    project: core.getInput('project', { required: false }),
    configuration: core.getInput('configuration', { required: false }),
    framework: core.getInput('framework', { required: false }),
    runtime: core.getInput('runtime', { required: false }),
    nuget_source: core.getInput('nuget-source', { required: false }),
    nuget_packages_dir: core.getInput('nuget-packages-dir', { required: false }),
    nuget_configfile: core.getInput('nuget-configfile', { required: false }),
    dotnet_arguments: (_a = core.getInput('dotnet-arguments', { required: false })) === null || _a === void 0 ? void 0 : _a.split(os_1.EOL),
    msbuild_arguments: (_b = core.getInput('msbuild-arguments', { required: false })) === null || _b === void 0 ? void 0 : _b.split(os_1.EOL),
    verbosity: core.getInput('verbosity', { required: false }),
    binlogDir: core.getInput('binary-log-directory', { required: false })
};
console.log(inputs);
glob.create(inputs.project).then(function (globber) {
    var projGlobs = inputs.project.split(os_1.EOL);
    var projGlobMessage = 'Input project glob patterns:';
    if (projGlobs.length) {
        for (var projGlobPattern in projGlobs) {
            projGlobMessage += os_1.EOL + '  - ' + projGlobPattern;
        }
    }
    else {
        projGlobMessage += ' []';
    }
    core.info(projGlobMessage);
    globber.glob().then(function (projGlobResults) {
        var projGlobResultMessage = 'Resolved project glob paths:';
        if (projGlobResults.length) {
            for (var projGlobResultPath in projGlobResults) {
                projGlobResultMessage += os_1.EOL + '  - ' + projGlobResultPath;
            }
        }
        else {
            projGlobResultMessage += ' []';
        }
        core.info(projGlobResultMessage);
    });
});

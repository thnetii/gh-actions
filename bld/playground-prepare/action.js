const core = require('@actions/core');

try {
  core.setOutput('generated', [{
    id: 'doNothing',
    name: 'Do nothing',
    uses: 'thnetii/gh-actions/bld/noop@master'
  }, {
    id: 'doNothingLocal',
    name: 'Do nothing (repository local)',
    uses: './bld/noop'
  }]);
} catch (error) {
  core.setFailed(error);
}

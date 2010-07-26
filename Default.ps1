
properties {
  $project_name = 'QueryExamples'
  $configuration = 'Release'
  $msbuild_verbosity = 'quiet'
  $build_dir = Split-Path $psake.build_script_file
  $solution_file = Join-Path $build_dir "$project_name.sln"
  $build_artifacts_dir = Join-Path $build_dir "Build"
}

task default -depends Clean, Build

task Clean {
  if (Test-Path $build_artifacts_dir) {
    Remove-Item $build_artifacts_dir -Recurse -Force | Out-Null
  }
  
  New-Item -Path $build_artifacts_dir -Type Directory | Out-Null
  
  Exec { msbuild $solution_file /t:Clean `
                                /p:Configuration=$configuration `
                                /v:$msbuild_verbosity }
}

task Build {
  Exec { msbuild $solution_file /t:Build `
                                /p:Configuration=$configuration `
                                /p:OutDir="$build_artifacts_dir\" `
                                /v:$msbuild_verbosity }
}
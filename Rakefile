require 'rake'
require 'albacore'

task :default => [:xbuild, :run]

xbuild do |msb|
  msb.properties :configuration => :Debug
  msb.targets :Clean, :Build
  msb.solution = 'QueryExamples.sln'
end

task :run do
  sh 'QueryExamples/bin/Debug/QueryExamples.exe'
end

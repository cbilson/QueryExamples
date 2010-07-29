require 'rake'
require 'albacore'

xbuild do |msb|
  msb.properties :configuration => :Debug
  msb.targets :Clean, :Build
  msb.solution = 'QueryExamples.sln'
end

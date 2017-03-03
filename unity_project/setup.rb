#!/usr/bin/env ruby
require 'rake'
require 'securerandom'

username = 'netpyoung'
reponame = 'nf.template'
temp_zip = "#{SecureRandom.uuid}_#{reponame}.zip"
new_project = 'unity_project'

sh <<EOS
wget -O #{temp_zip} https://github.com/#{username}/#{reponame}/archive/master.zip && \
unzip #{temp_zip} && \
rm #{temp_zip} && \
mv #{reponame}-master #{new_project} && \
cd #{new_project} && \
git init
EOS

require 'json'
require 'fileutils'

BUILD_DIR = '__BUILD'

task :default => :auto_generate_dlls


task :ensure_build_dir do
  mkdir_p BUILD_DIR
end

desc "auto genreate_dlls"
task :auto_generate_dlls => [:ensure_build_dir] do
  # dependencies for protoc
  # ref: https://developers.google.com/protocol-buffers/docs/downloads
  # brew install protobuf # for macOs

  # % it isn't required.%
  # sh 'git clone https://github.com/google/protobuf.git'
  # Dir.chdir('protobuf/csharp/src/Google.Protobuf') do
  #   contents = File.read('project.json')
  #   my_hash = JSON.parse(contents)
  #   my_hash['frameworks'] = {"net35" => {}}
  #   File.write('project.json', my_hash.to_json)

  #   sh 'dotnet restore'
  #   sh 'dotnet build --configuration Debug'
  # end


  Dir.chdir('NF.Network.Protocol.Interface') do
    sh 'dotnet restore'
    sh "dotnet build"
  end

  Dir.chdir('NF.Network.Transfer.Protobuf') do
    sh 'dotnet restore'
    sh "dotnet build"
  end

  message_dir = "@MESSAGE"
  output_dir = "@CUSTOM/AutoGenerated.Message/output"
  mkdir_p output_dir
  sh "protoc -I=#{message_dir} --csharp_out=#{output_dir} #{message_dir}/*.proto"

  Dir.chdir('@CUSTOM/AutoGenerated.Message') do
    sh 'dotnet restore'
    sh "dotnet build"
  end

  dll = File.expand_path("@CUSTOM/AutoGenerated.Message/bin/Debug/net35/AutoGenerated.Message.dll")
  interface_template = File.expand_path("@CUSTOM/template/interface.liquid")
  interface_output = File.expand_path("@CUSTOM/AutoGenerated.Interface/out")
  transfer_template = File.expand_path("@CUSTOM/template/transfer.liquid")
  transfer_output = File.expand_path("@CUSTOM/AutoGenerated.Transfer/out")

  Dir.chdir('NF.CLI.ProtocolGenerator') do
    sh 'dotnet restore'
    sh "dotnet run -- --dll #{dll} --interface_template #{interface_template} --interface_output #{interface_output} --transfer_template #{transfer_template} --transfer_output #{transfer_output}"
  end

  Dir.chdir('@CUSTOM/AutoGenerated.Interface') do
    sh 'dotnet restore'
    sh "dotnet build"
  end

  Dir.chdir('@CUSTOM/AutoGenerated.Transfer') do
    sh 'dotnet restore'
    sh "dotnet build"
  end


  FileUtils.cp_r Dir.glob('@CUSTOM/AutoGenerated.Transfer/bin/Debug/net35/*.dll'), BUILD_DIR
end


desc "autogenerated compact one.dll"
task :auto_one => [:auto_generate_dlls] do
  Dir.chdir(BUILD_DIR) do
    sh 'nuget install ILRepack -Version 2.0.12'
    exe = 'ILRepack.2.0.12/tools/ILRepack.exe'

    is_macOs = ((/darwin/ =~ RUBY_PLATFORM) != nil)
    exe = "mono #{exe}" if is_macOs

    output_dll = 'NF_PROTOCOL.dll'
    sh "#{exe} /out:#{output_dll} AutoGenerated.Interface.dll AutoGenerated.Message.dll AutoGenerated.Transfer.dll NF.Network.Protocol.Interface.dll NF.Network.Transfer.Protobuf.dll protobuf3.dll"
  end
end


desc "run test server"
task :test_server do
  # pip install protobuf
  message_dir = "@MESSAGE"
  output_dir = "test_server"

  mkdir_p output_dir
  sh "protoc -I=#{message_dir} --python_out=#{output_dir} #{message_dir}/*.proto"
  Dir.chdir(output_dir) do
    sh "python main.py"
  end
end

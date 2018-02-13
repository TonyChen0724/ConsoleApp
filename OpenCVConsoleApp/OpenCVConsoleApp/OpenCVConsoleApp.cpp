// OpenCVConsoleApp.cpp : Defines the entry point for the console application.
//
#define _USE_MATH_DEFINES
#include "stdafx.h"
#include "FisheyeModel.h"
#include "GPMFInterface.h"
#include "FileManager.h"

#include <sstream>
#include <iterator>
#include <locale>
#include <codecvt>
#include <sys/types.h>
#include <sys/stat.h>
#include <direct.h>
//#include <tbb\tbb.h>
//#include <boost\filesystem\string_file.hpp>
//using namespace tbb;
GPMFInterface gpmf_interface;
FileManager g_file_manager;
fisheyeTest fisheye_test;
std::vector<std::string> split(const std::string &s, char delim);
void ProcessFolder(int idx, char * inFolder)
{
	bool Result = false;
	WIN32_FIND_DATAW Data;
	//CString folder(inFolder);
	//if (folder.GetLength() < 4)
	//	return;
	//CString command = folder;
	//command.Append("*.*");
	std::string folder(inFolder);

	std::cout << folder << std::endl;

	const size_t cSize = strlen(inFolder) + 1;
	wchar_t* wc = new wchar_t[cSize];
	mbstowcs(wc, inFolder, cSize);
	
	std::wstring command(wc);
	command.append(L"*.*");
	HANDLE Handle = FindFirstFile(command.c_str(), &Data);
	if (Handle != INVALID_HANDLE_VALUE)
	{
		std::wstring outFolder(wc);
		outFolder.append(L"Output/");
		CreateDirectory(outFolder.c_str(), NULL);
		Result = true;
		do
		{
			std::wstring filename(Data.cFileName);
			CHAR tempChar[MAX_PATH];
			wcstombs(tempChar, Data.cFileName, MAX_PATH);
			std::string filename_c(tempChar);
			if (filename.compare(L".") && filename.compare(L".."))
			{
				int len = filename.length();
				if (len < 5)
					continue;
				wchar_t c1, c2, c3, c4;
				c1 = filename.at(len - 3);
				c2 = filename.at(len - 2);
				c3 = filename.at(len - 1);
				c4 = filename.at(len - 4);
				if (c1 == L'j' || c1 == L'J')
				{
					if (c2 == L'p' || c2 == L'P')
					{
						if (c3 == L'g' || c3 == L'G')
						{
							std::string inFilename = folder;
							inFilename.append(filename_c.c_str());
							std::string outputFilename = folder;
							outputFilename.append("Output/");
							outputFilename.append(filename_c.c_str());
						    //fisheye_test->CopyImage(inFilename.GetBuffer(), outputFilename.GetBuffer());
							fisheye_test.undistortImage(idx, inFilename.c_str(), outputFilename.c_str());

							std::cout << "Output:  " << outputFilename.c_str() << std::endl;
						}
					}
				}				
			}
		} while (Result && FindNextFileW(Handle, &Data));
		FindClose(Handle);
	}
	return;
}

void BuildCubeMapFromFisheye200(char **argv)
{
	char *infolder = argv[2];
	char *storepath = argv[3];
	int faceSize = atoi(argv[4]);
	/*int startSerial = atoi(argv[5]);
	int endSerial = atoi(argv[6]);*/

	int width = faceSize;
	int height = faceSize;

	std::string storePath(storepath);
	std::string folder(infolder);
	std::cout << folder << std::endl;
	const size_t cSize = strlen(infolder) + 1;
	wchar_t* wc = new wchar_t[cSize];
	mbstowcs(wc, infolder, cSize);
	std::wstring command(wc);
	command.append(L"*.*");

	WIN32_FIND_DATA data;
	HANDLE Handle = FindFirstFile(command.c_str(), &data);
	if (Handle != INVALID_HANDLE_VALUE)
	{
		std::wstring outFolder(wc);
		std::string outImgFolder[6];
		std::wstring outFrontFolder = outFolder;
		outFrontFolder.append(L"Front/");
		CreateDirectory(outFrontFolder.c_str(), NULL);
		std::wstring outRightFolder = outFolder;
		outRightFolder.append(L"Right/");
		CreateDirectory(outRightFolder.c_str(), NULL);
		std::wstring outLeftFolder = outFolder;
		outLeftFolder.append(L"Left/");
		CreateDirectory(outLeftFolder.c_str(), NULL);
		/*std::wstring outTopFolder = outFolder;
		outTopFolder.append(L"Top/");
		CreateDirectory(outTopFolder.c_str(), NULL);
		std::wstring outButtomFolder = outFolder;
		outButtomFolder.append(L"Buttom/");
		CreateDirectory(outButtomFolder.c_str(), NULL);*/
		CHAR tempChar[MAX_PATH];
		wcstombs(tempChar, outFrontFolder.c_str(), MAX_PATH);
		outImgFolder[0] = std::string(tempChar);
		CHAR tempChar1[MAX_PATH];
		wcstombs(tempChar1, outRightFolder.c_str(), MAX_PATH);
		outImgFolder[1] = std::string(tempChar1);
		CHAR tempChar2[MAX_PATH];
		wcstombs(tempChar2, outLeftFolder.c_str(), MAX_PATH);
		outImgFolder[2] = std::string(tempChar2);
	/*	CHAR tempChar3[MAX_PATH];
		wcstombs(tempChar3, outTopFolder.c_str(), MAX_PATH);
		outImgFolder[3] = std::string(tempChar3);
		CHAR tempChar4[MAX_PATH];
		wcstombs(tempChar4, outButtomFolder.c_str(), MAX_PATH);
		outImgFolder[4] = std::string(tempChar4);*/

		bool isMappingMat = false;
		cv::Mat MappingX[6];
		cv::Mat MappingY[6];
		cv::Mat inFace;

		do
		{
			std::wstring filename(data.cFileName);
			CHAR tempChar[MAX_PATH];
			wcstombs(tempChar, data.cFileName, MAX_PATH);
			std::string filename_c(tempChar);
			if (filename.compare(L".") && filename.compare(L".."))
			{
				int len = filename.length();
				if (len < 5)
					continue;
				wchar_t c1, c2, c3, c4;
				c1 = filename.at(len - 3);
				c2 = filename.at(len - 2);
				c3 = filename.at(len - 1);
				c4 = filename.at(len - 4);
				if (c1 == L'j' || c1 == L'J')
				{
					if (c2 == L'p' || c2 == L'P')
					{
						if (c3 == L'g' || c3 == L'G')
						{
							std::string inFilename = folder;
							inFilename.append(filename_c.c_str());
							std::string firstPart = split(filename_c, '.')[0];
							std::string idxStr = split(firstPart, '_')[1];
							std::string outputfilename[6];
							outputfilename[0] = outImgFolder[0] + "Front_" + idxStr + ".jpg";
							outputfilename[1] = outImgFolder[1] + "Right_" + idxStr + ".jpg";
							outputfilename[2] = outImgFolder[2] + "Left_" + idxStr + ".jpg";
							//outputfilename[3] = outImgFolder[3] + "Top_" + idxStr + ".jpg";
							//outputfilename[4] = outImgFolder[4] + "Buttom_" + idxStr + ".jpg";

							IplImage * image = cvLoadImage(inFilename.c_str(), CV_LOAD_IMAGE_COLOR);
							cv::Mat inImg(image);
							
							
							if (!isMappingMat)
							{
								inFace = cv::Mat(height, width * 2, inImg.type());
								
								fisheye_test.fish2sphere(inImg, MappingX[0], MappingY[0], 220);
								//fisheye_test.createCubeMapMappingFromFisheye200(inImg, MappingX[0], MappingY[0], 3, faceSize, faceSize);
								
								
								//fisheye_test.createCubeMapMappingFromFisheye200(inImg, MappingX[1], MappingY[1], 4, faceSize, faceSize);
								//fisheye_test.createCubeMapMappingFromFisheye200(inImg, MappingX[2], MappingY[2], 5, faceSize, faceSize);
								
								
								//fisheye_test.createCubeMapMappingFromFisheye200(inImg, MappingX[3], MappingY[3], 4, faceSize, faceSize);
								//fisheye_test.createCubeMapMappingFromFisheye200(inImg, MappingX[4], MappingY[4], 5, faceSize, faceSize);
								isMappingMat = true;

								std::cout << "Create Cubemap mapping from fisheye 200:  " << inFilename.c_str() << std::endl;
							}

							std::vector<int> compression_params;
							compression_params.push_back(CV_IMWRITE_JPEG_QUALITY);
							compression_params.push_back(100);
							
							remap(inImg, inFace, MappingX[0], MappingY[0], CV_INTER_LINEAR, cv::BORDER_CONSTANT, cv::Scalar(0, 0, 0));
							cv::imwrite(outputfilename[0], inFace, compression_params);
							
							/*remap(inImg, inFace, MappingX[1], MappingY[1], CV_INTER_LINEAR, cv::BORDER_CONSTANT, cv::Scalar(0, 0, 0));
							cv::imwrite(outputfilename[1], inFace);
							remap(inImg, inFace, MappingX[2], MappingY[2], CV_INTER_LINEAR, cv::BORDER_CONSTANT, cv::Scalar(0, 0, 0));
							cv::imwrite(outputfilename[2], inFace);
*/

							/*remap(inImg, inFace, MappingX[3], MappingY[3], CV_INTER_LINEAR, cv::BORDER_CONSTANT, cv::Scalar(0, 0, 0));
							cv::imwrite(outputfilename[3], inFace);
							remap(inImg, inFace, MappingX[4], MappingY[4], CV_INTER_LINEAR, cv::BORDER_CONSTANT, cv::Scalar(0, 0, 0));
							cv::imwrite(outputfilename[4], inFace);*/
							

							std::cout << "Output Cubemap from fisheye 200:  " << inFilename.c_str() << std::endl;
						}
					}
				}
			}
		} 
		while (FindNextFileW(Handle, &data));
		FindClose(Handle);


		inFace.release();
	}
	return;
}

void ExtractImageAndCubeMaps(char **argv) {

	WIN32_FIND_DATA data;

	char *infolder = argv[2];
	char *storepath = argv[3];
	int faceSize = atoi(argv[4]);
	int startSerial = atoi(argv[5]);
	int endSerial = atoi(argv[6]);
	
	std::string storePath(storepath);
	std::string folder(infolder);
	std::cout << folder << std::endl;

	const size_t cSize = strlen(infolder) + 1;
	wchar_t* wc = new wchar_t[cSize];
	mbstowcs(wc, infolder, cSize);

	std::wstring command(wc);
	command.append(L"*.*");

	//std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
	//std::string narrow = converter.to_bytes(wide_utf16_source_string);
	//std::wstring widefolder = converter.from_bytes(folderName);

	//HANDLE hFind = FindFirstFile((LPCWSTR)folder, &data);
	HANDLE hFind = FindFirstFile(command.c_str(), &data);
	if (hFind != INVALID_HANDLE_VALUE) {
		int j = 0;

		struct stat st = { 0 };
		if (stat(storePath.c_str(), &st) == -1) {
			_mkdir(storePath.c_str());
		}

		std::string outputPath = storePath + "\\Front\\";
		st = { 0 };
		if (stat(outputPath.c_str(), &st) == -1) {
			_mkdir(outputPath.c_str());
		}
		
		outputPath = storePath + "\\Right\\";
		st = { 0 };
		if (stat(outputPath.c_str(), &st) == -1) {
			_mkdir(outputPath.c_str());
		}

		outputPath = storePath + "\\Back\\";
		st = { 0 };
		if (stat(outputPath.c_str(), &st) == -1) {
			_mkdir(outputPath.c_str());
		}

		outputPath = storePath + "\\Left\\";
		st = { 0 };
		if (stat(outputPath.c_str(), &st) == -1) {
			_mkdir(outputPath.c_str());
		}

		//outputPath = storePath + "\\Top\\";
		//st = { 0 };
		//if (stat(outputPath.c_str(), &st) == -1) {
		//	_mkdir(outputPath.c_str());
		//}

		//outputPath = storePath + "\\Down\\";
		//st = { 0 };
		//if (stat(outputPath.c_str(), &st) == -1) {
		//	_mkdir(outputPath.c_str());
		//}
		bool isMappingMat = false;
		cv::Mat MappingX[6];
		cv::Mat MappingY[6];
		cv::Mat inFace;

		do {
			j++;
			if (j == 1 || j == 2) continue;
			CHAR tempChar[MAX_PATH];
			wcstombs(tempChar, data.cFileName, MAX_PATH);
			std::string filename_c(tempChar); //filename_c
			std::string fileAddress = folder + filename_c;
			const char *fileInfo = fileAddress.c_str();
			const char *filename = filename_c.c_str();
			//std::cout << filename << std::endl;

			int len = filename_c.length();
			if (len < 5)
				continue;

			IplImage * image = cvLoadImage(fileInfo, CV_LOAD_IMAGE_COLOR);
			cv::Mat inImg(image);
			cv::Mat inFace;
			std::string outputfileName;
			std::string location;


			wchar_t c1, c2, c3, c4;
			c1 = filename_c.at(len - 3);
			c2 = filename_c.at(len - 2);
			c3 = filename_c.at(len - 1);
			c4 = filename_c.at(len - 4);
			if (c1 == 'j' || c1 == 'J')
			{
				if (c2 == 'p' || c2 == 'P')
				{
					if (c3 == 'g' || c3 == 'G')
					{
						std::string firstPart = split(filename_c, '.')[0];
						std::vector<std::string> tempSplitStr = split(firstPart, '_');
						if (tempSplitStr.size() < 2)
							continue;
						std::string serialNumber = split(firstPart, '_')[1];
						int idx = atoi(serialNumber.c_str());
						if (idx < startSerial || idx > endSerial)
							continue;

						if (!isMappingMat)
						{
							inFace = cv::Mat(faceSize, faceSize, inImg.type());

							fisheye_test.createCubeMapFace(inImg, MappingX[0], MappingY[0], 0, faceSize, faceSize);
							fisheye_test.createCubeMapFace(inImg, MappingX[1], MappingY[1], 1, faceSize, faceSize);
							fisheye_test.createCubeMapFace(inImg, MappingX[2], MappingY[2], 2, faceSize, faceSize);
							fisheye_test.createCubeMapFace(inImg, MappingX[3], MappingY[3], 3, faceSize, faceSize);

							isMappingMat = true;

							std::cout << "Create Cubemap mapping from spherical image:  " << filename_c.c_str() << std::endl;
						}


						std::vector<int> compression_params;
						compression_params.push_back(CV_IMWRITE_JPEG_QUALITY);
						compression_params.push_back(100);

						remap(inImg, inFace, MappingX[0], MappingY[0], CV_INTER_LINEAR, cv::BORDER_CONSTANT, cv::Scalar(0, 0, 0));
						outputfileName = storePath + "\\Front\\" + "Front_" + serialNumber + ".jpg";
						cv::imwrite(outputfileName, inFace, compression_params);

						remap(inImg, inFace, MappingX[1], MappingY[1], CV_INTER_LINEAR, cv::BORDER_CONSTANT, cv::Scalar(0, 0, 0));
						outputfileName = storePath + "\\Right\\" + "Right_" + serialNumber + ".jpg";
						cv::imwrite(outputfileName, inFace, compression_params);

						remap(inImg, inFace, MappingX[2], MappingY[2], CV_INTER_LINEAR, cv::BORDER_CONSTANT, cv::Scalar(0, 0, 0));
						outputfileName = storePath + "\\Back\\" + "Back_" + serialNumber + ".jpg";
						cv::imwrite(outputfileName, inFace, compression_params);

						remap(inImg, inFace, MappingX[3], MappingY[3], CV_INTER_LINEAR, cv::BORDER_CONSTANT, cv::Scalar(0, 0, 0));
						outputfileName = storePath + "\\Left\\" + "Left_" + serialNumber + ".jpg";
						cv::imwrite(outputfileName, inFace, compression_params);

						std::cout << "Output Cubemap from spherical images:  " << filename_c.c_str() << std::endl;
					}
				}
			}
			
			inFace.release();
			inImg.release();
			cvReleaseImage(&image);

		} while (FindNextFile(hFind, &data));

		FindClose(hFind);
	}
	//std::cin.get();
	
}



std::string g_image_ext[2] = { "jpg","JPG" };

void BatchMergeVirb360Images(char * inFolder,int startIdx,int endIdx)
{
	fisheye_test.InitLog();

	g_file_manager.SetDestFolder(inFolder);

	g_file_manager.ProcessFolder(2, g_image_ext);

	g_file_manager.CreateFolder(L"OutputVirb");

	std::map<int, std::string>::iterator it = g_file_manager.file_map.begin();
	std::map<int, std::string>::iterator itEnd = g_file_manager.file_map.end();
	for (;it!=itEnd;it++)
	{
		int idx = it->first;
		if (idx < startIdx)
			continue;
		if (idx > endIdx)
			continue;
		std::string filename = it->second;

		std::string inFilename = g_file_manager.dest_folder;
		inFilename.append(filename.c_str());
		std::string outputFilename = g_file_manager.dest_folder;
		outputFilename.append("OutputVirb/");
		outputFilename.append(filename.c_str());

		fisheye_test.MergeVirb360Image(inFilename.c_str(), outputFilename.c_str());
		
		std::cout << "Output Virb360:  " << outputFilename.c_str() << std::endl;
	}

	std::string outputFilename = g_file_manager.dest_folder;
	outputFilename.append("OutputVirb/");
	fisheye_test.BuildLogFile(outputFilename.c_str(), 2);
}

void MergeVirb360Images(char * inFolder)
{
	fisheye_test.InitLog();

	g_file_manager.SetDestFolder(inFolder);

	g_file_manager.ProcessFolder(2, g_image_ext);

	g_file_manager.CreateFolder(L"OutputVirb");

	std::map<int, std::string>::iterator it = g_file_manager.file_map.begin();
	std::map<int, std::string>::iterator itEnd = g_file_manager.file_map.end();
	for (; it != itEnd; it++)
	{
		int idx = it->first;
		std::string filename = it->second;

		std::string inFilename = g_file_manager.dest_folder;
		inFilename.append(filename.c_str());
		std::string outputFilename = g_file_manager.dest_folder;
		outputFilename.append("OutputVirb/");
		outputFilename.append(filename.c_str());

		fisheye_test.MergeVirb360Image(inFilename.c_str(), outputFilename.c_str());

		std::cout << "Output Virb360:  " << outputFilename.c_str() << std::endl;
	}

	std::string outputFilename = g_file_manager.dest_folder;
	outputFilename.append("OutputVirb/");
	fisheye_test.BuildLogFile(outputFilename.c_str(), 2);

	//fisheye_test.InitLog();
	//bool Result = false;
	//WIN32_FIND_DATAW Data;
	//std::string folder(inFolder);
	//folder.append("\\");
	//std::cout << folder << std::endl;

	//const size_t cSize = strlen(inFolder) + 1;
	//wchar_t* wc = new wchar_t[cSize];
	//mbstowcs(wc, inFolder, cSize);

	//std::wstring command(wc);
	//command.append(L"\\*.*");
	//HANDLE Handle = FindFirstFile(command.c_str(), &Data);
	//if (Handle != INVALID_HANDLE_VALUE)
	//{
	//	std::wstring outFolder(wc);
	//	outFolder.append(L"/OutputVirb/");
	//	CreateDirectory(outFolder.c_str(), NULL);
	//	Result = true;
	//	do
	//	{
	//		std::wstring filename(Data.cFileName);
	//		CHAR tempChar[MAX_PATH];
	//		wcstombs(tempChar, Data.cFileName, MAX_PATH);
	//		std::string filename_c(tempChar);
	//		if (filename.compare(L".") && filename.compare(L".."))
	//		{
	//			int len = filename.length();
	//			if (len < 5)
	//				continue;
	//			wchar_t c1, c2, c3, c4;
	//			c1 = filename.at(len - 3);
	//			c2 = filename.at(len - 2);
	//			c3 = filename.at(len - 1);
	//			c4 = filename.at(len - 4);
	//			if (c1 == L'j' || c1 == L'J')
	//			{
	//				if (c2 == L'p' || c2 == L'P')
	//				{
	//					if (c3 == L'g' || c3 == L'G')
	//					{
	//						std::string inFilename = folder;
	//						inFilename.append(filename_c.c_str());
	//						std::string outputFilename = folder;
	//						outputFilename.append("OutputVirb/");
	//						outputFilename.append(filename_c.c_str());
	//						//fisheye_test->CopyImage(inFilename.GetBuffer(), outputFilename.GetBuffer());
	//						fisheye_test.MergeVirb360Image(inFilename.c_str(), outputFilename.c_str());

	//						std::cout << "Output:  " << outputFilename.c_str() << std::endl;
	//					}
	//				}
	//			}
	//		}
	//	} while (Result && FindNextFileW(Handle, &Data));
	//	FindClose(Handle);
	//}
	//std::string outputFilename = folder;
	//outputFilename.append("OutputVirb/");
	//fisheye_test.BuildLogFile(outputFilename.c_str(), 2);
	return;
}


void ChangeResolutionWithParams(char * inFolder, int targetResolutionX,int targetResolutionY)
{

	fisheye_test.InitLog();
	bool Result = false;
	WIN32_FIND_DATAW Data;
	std::string folder(inFolder);
	folder.append("\\");
	std::cout << folder << std::endl;

	const size_t cSize = strlen(inFolder) + 1;
	wchar_t* wc = new wchar_t[cSize];
	mbstowcs(wc, inFolder, cSize);

	std::wstring command(wc);
	command.append(L"\\*.*");
	HANDLE Handle = FindFirstFile(command.c_str(), &Data);
	if (Handle != INVALID_HANDLE_VALUE)
	{
		std::wstring outFolder(wc);
		outFolder.append(L"/Output/");
		CreateDirectory(outFolder.c_str(), NULL);
		Result = true;
		do
		{
			std::wstring filename(Data.cFileName);
			CHAR tempChar[MAX_PATH];
			wcstombs(tempChar, Data.cFileName, MAX_PATH);
			std::string filename_c(tempChar);
			if (filename.compare(L".") && filename.compare(L".."))
			{
				int len = filename.length();
				if (len < 5)
					continue;
				wchar_t c1, c2, c3, c4;
				c1 = filename.at(len - 3);
				c2 = filename.at(len - 2);
				c3 = filename.at(len - 1);
				c4 = filename.at(len - 4);
				if (c1 == L'j' || c1 == L'J')
				{
					if (c2 == L'p' || c2 == L'P')
					{
						if (c3 == L'g' || c3 == L'G')
						{
							std::string inFilename = folder;
							inFilename.append(filename_c.c_str());
							std::string outputFilename = folder;
							outputFilename.append("Output/");
							outputFilename.append(filename_c.c_str());
							//fisheye_test->CopyImage(inFilename.GetBuffer(), outputFilename.GetBuffer());
							fisheye_test.ChangeResolution(inFilename.c_str(), outputFilename.c_str(), targetResolutionX, targetResolutionY);

							std::cout << "Output:  " << outputFilename.c_str() << std::endl;
						}
					}
				}
			}
		} while (Result && FindNextFileW(Handle, &Data));
		FindClose(Handle);
	}
	std::string outputFilename = folder;
	outputFilename.append("Output/");
	fisheye_test.BuildLogFile(outputFilename.c_str(),2);
	return;
}




void ProcessFolderWithParams(int idx, char * inFolder,int isBackup)
{
	fisheye_test.InitLog();
	bool Result = false;
	WIN32_FIND_DATAW Data;
	//CString folder(inFolder);
	//if (folder.GetLength() < 4)
	//	return;
	//CString command = folder;
	//command.Append("*.*");
	std::string folder(inFolder);
	folder.append("\\");
	std::cout << folder << std::endl;

	const size_t cSize = strlen(inFolder) + 1;
	wchar_t* wc = new wchar_t[cSize];
	mbstowcs(wc, inFolder, cSize);

	std::wstring command(wc);
	command.append(L"\\*.*");
	HANDLE Handle = FindFirstFile(command.c_str(), &Data);
	if (Handle != INVALID_HANDLE_VALUE)
	{
		std::wstring outFolder(wc);
		//if(isBackup)
		//outFolder.append(L"Output/");
		CreateDirectory(outFolder.c_str(), NULL);

		if (isBackup == 1)
		{
			std::wstring backupFolder(wc);
			backupFolder.append(L"/UndistortBackup/");
			CreateDirectory(backupFolder.c_str(), NULL);
			std::string srcFolder = folder;
			std::string command = "copy ";
			command.append("\"");
			command.append(srcFolder);
			command.append("*.jpg\" \"");
			command.append(srcFolder);
			command.append("UndistortBackup\\");
			command.append("\"");
			system(command.c_str());
		}
		Result = true;
		do
		{
			std::wstring filename(Data.cFileName);
			CHAR tempChar[MAX_PATH];
			wcstombs(tempChar, Data.cFileName, MAX_PATH);
			std::string filename_c(tempChar);
			if (filename.compare(L".") && filename.compare(L".."))
			{
				int len = filename.length();
				if (len < 5)
					continue;
				wchar_t c1, c2, c3, c4;
				c1 = filename.at(len - 3);
				c2 = filename.at(len - 2);
				c3 = filename.at(len - 1);
				c4 = filename.at(len - 4);
				if (c1 == L'j' || c1 == L'J')
				{
					if (c2 == L'p' || c2 == L'P')
					{
						if (c3 == L'g' || c3 == L'G')
						{
							std::string inFilename = folder;
							inFilename.append(filename_c.c_str());
							std::string outputFilename = folder;
							//outputFilename.append("Output/");
							outputFilename.append(filename_c.c_str());
							//fisheye_test->CopyImage(inFilename.GetBuffer(), outputFilename.GetBuffer());
							fisheye_test.undistortImageWithCamConfig(idx, inFilename.c_str(), outputFilename.c_str());

							std::cout << "Output:  " << outputFilename.c_str() << std::endl;
						}
					}
				}
			}
		} while (Result && FindNextFileW(Handle, &Data));
		FindClose(Handle);
	}

	std::string outputFilename = folder;
	outputFilename.append("Output/");
	fisheye_test.BuildLogFile(outputFilename.c_str(),1);
	return;
}

template <typename Out>

void split(const std::string &s, char delim, Out result) {
	std::stringstream ss(s);
	std::string item;
	while (std::getline(ss, item, delim)) {
		*(result++) = item;
	}
}

std::vector<std::string> split(const std::string &s, char delim) {
	std::vector<std::string> elems;
	split(s, delim, std::back_inserter(elems));
	return elems;
}



int main(int argc, char* argv[])
{
	if (argc < 9)
	{
		if (argc > 2)
		{
			int nCode = atoi(argv[1]);
			if (nCode == 2 && argc == 4)
			{
				int res = atoi(argv[3]);
				ChangeResolutionWithParams(argv[2],res, res);
			}
			if (nCode == 3)
			{
				if (argc == 5)
				{
					int i1 = atoi(argv[3]);
					int i2 = atoi(argv[4]);
					BatchMergeVirb360Images(argv[2],i1,i2);
				}
				else
					MergeVirb360Images(argv[2]);
			}			
			if (nCode == 2 && argc == 5)
			{
				int resX = atoi(argv[3]);
				int resY = atoi(argv[4]);
				ChangeResolutionWithParams(argv[2], resX, resY);
			}
			if (nCode == 10)
			{
				gpmf_interface.Run(argv[2], argv[3]);
			}
			
			// tstcube
			if (nCode == 11) {			
				ExtractImageAndCubeMaps(argv);
				
			}

			if(nCode == 12)
				BuildCubeMapFromFisheye200(argv);

			return 0;
			
		}
		
		/*std::vector<cv::Mat> input;
		tbb::parallel_for(size_t(0), input.size(), size_t(1), [=](size_t i) {
			fisheye_test.MergeVirb360Image("","");
		});*/


		//	tbb::parallel_for(size_t(0), input.size(), size_t(1), [=](size_t i) {
		//	processBinary(input[i]);
		//});

		fisheye_test.LoadConfig("Config.ini");
		ProcessFolder(0, fisheye_test.m_process_directory1);
		ProcessFolder(1, fisheye_test.m_process_directory2);
		ProcessFolder(2, fisheye_test.m_process_directory3);
		ProcessFolder(3, fisheye_test.m_process_directory4);


	}
	
	fisheye_test.m_cam_config.focallen = (float)atof(argv[2]);
	fisheye_test.m_cam_config.r1 = (float)atof(argv[3]);
	fisheye_test.m_cam_config.r2 = (float)atof(argv[4]);
	fisheye_test.m_cam_config.r3 = (float)atof(argv[5]);
	fisheye_test.m_cam_config.ppx = (float)atof(argv[6]);
	fisheye_test.m_cam_config.ppy = (float)atof(argv[7]);
	fisheye_test.m_dest_focal_length = (float)atof(argv[8]);
	int isBackUP = atoi(argv[9]);
	ProcessFolderWithParams(0, argv[1], isBackUP);
    return 0;
}


#include "stdafx.h"
#include "FileManager.h"


FileManager::FileManager()
{

}

FileManager::~FileManager()
{

}


void FileManager::SetDestFolder(char * folder)
{
	dest_folder = std::string(folder);
	dest_folder.append("\\");
	const size_t cSize = strlen(dest_folder.c_str()) + 1;
	wchar_t* wc = new wchar_t[cSize];
	mbstowcs(wc, dest_folder.c_str(), cSize);
	dest_folderW = std::wstring(wc);
	return;
}

void FileManager::CreateFolder(wchar_t * folderName)
{	
	std::wstring folder = dest_folderW;
	folder.append(folderName);
	folder.append(L"/");
	CreateDirectory(folder.c_str(), NULL);
	return;
}

std::string FileManager::GetExtension(std::string &filename)
{
	int dot = filename.find_last_of('.');
	return filename.substr(dot + 1);
}

int FileManager::GetKeywordIndex(std::string &filename)
{
	int dot = filename.find_last_of('.');
	std::string filename2 = filename.substr(0,dot);
	int underline = filename2.find_last_of('_');
	std::string idxStr = filename2.substr(underline + 1);
	if (idxStr.length() < 1)
		return -1;
	return atoi(idxStr.c_str());
}


void FileManager::ProcessFolder(int nbExt, std::string* exts)
{
	file_map.clear();
	bool Result = false;
	WIN32_FIND_DATAW Data;
	const size_t cSize = strlen(dest_folder.c_str()) + 1;
	wchar_t* wc = new wchar_t[cSize];
	mbstowcs(wc, dest_folder.c_str(), cSize);
	std::wstring command(wc);
	command.append(L"*.*");
	HANDLE Handle = FindFirstFile(command.c_str(), &Data);
	if (Handle != INVALID_HANDLE_VALUE)
	{
		Result = true;
		do
		{
			//std::wstring filename(Data.cFileName);
			CHAR tempChar[MAX_PATH];
			wcstombs(tempChar, Data.cFileName, MAX_PATH);
			std::string filename_c(tempChar);
			if (filename_c.compare(".") && filename_c.compare(".."))
			{
				int len = filename_c.length();
				if (len < 3)
					continue;

				std::string ext = GetExtension(filename_c);
				bool isFound = false;
				for (size_t i = 0; i < nbExt; i++)
				{
					if (ext.compare(exts[i]) == 0)
					{
						isFound = true;
						break;
					}					
				}
				if (!isFound)
					continue;
				int idx = GetKeywordIndex(filename_c);
				if (idx == -1)
					continue;
				file_map[idx] = filename_c;
			}
		} while (Result && FindNextFileW(Handle, &Data));
		
		FindClose(Handle);
	}
	return;
}
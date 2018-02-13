#pragma once


class FileManager
{
public:
	FileManager();
	~FileManager();

	void SetDestFolder(char * folder);
	void CreateFolder(wchar_t * folderName);

	static std::string GetExtension(std::string &filename);
	static int GetKeywordIndex(std::string &filename);

	void ProcessFolder(int nbExt,std::string* exts);
	std::string dest_folder;
	std::wstring dest_folderW;
	std::map<int, std::string> file_map;
private:
};
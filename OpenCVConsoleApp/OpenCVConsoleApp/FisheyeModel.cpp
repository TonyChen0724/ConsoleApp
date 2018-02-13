#include "stdafx.h"
#include <stdio.h>
#include <windows.h>
#include "FisheyeModel.h"
#include "PrivateProfile.h"
#include <iostream>
#include <fstream>
#include <time.h>

const cv::Size fisheyeTest::imageSize(2816, 1880);
const cv::Matx33d fisheyeTest::K(781.017, 0, 1407.9948730468750f,
	0, 521.418, 940.00329589843750,
	0, 0, 1);
//const cv::Vec<double, 5> fisheyeTest::D(-0.323936957188618, 0.119593, -0.000173, -0.000019, -0.021741);
const cv::Vec<double, 5> fisheyeTest::D(0.258093, 0.118122, 0, 0, -0.028720);
//const cv::Vec<double, 5> fisheyeTest::D(0, 0, 0, 0, 0);


const float g_NoHeightValue = -FLT_MAX;

cv::Mat fisheyeTest::mergeRectification(const cv::Mat& l, const cv::Mat& r)
{
	CV_Assert(l.type() == r.type() && l.size() == r.size());
	cv::Mat merged(l.rows, l.cols * 2, l.type());
	cv::Mat lpart = merged.colRange(0, l.cols);
	cv::Mat rpart = merged.colRange(l.cols, merged.cols);
	l.copyTo(lpart);
	r.copyTo(rpart);

	for (int i = 0; i < l.rows; i += 20)
		cv::line(merged, cv::Point(0, i), cv::Point(merged.cols, i), CV_RGB(0, 255, 0));

	return merged;
}

//f=781.0178f   8mm
//f=1253.289f  12.837499f   96.65
//f=1231.334f  12.6126f   97.658

#define PI 3.1415926f
float fisheyeTest::GetFocalLengthInPixel(float focallen,float width,float crop)
{
	static float factor = 43.2666153f;
	//float crop = 1.5f;
	float focalLength = focallen * crop;
	focalLength *= 2.f;

	fov = (2.f * atan(factor / focalLength)*180.f / PI);

	float focalLengthInPixels = (width* 0.5f) / tan(fov * 0.5f * PI / 180.f);
	return focalLengthInPixels;
}
float fisheyeTest::GetFocalLengthInPixelFrom36mmFocalLength(float focallen, float width)
{
	focallen *= 2.f;
	float fov = 2.f * atan(36.f / focallen)*180.f / PI;

	float focalLengthInPixels = (width* 0.5f) / tan(fov * 0.5f * PI / 180.f);
	return focalLengthInPixels;
}
void fisheyeTest::CopyImage(char * filename, char * output_filename)
{
	IplImage * image = cvLoadImage(filename, CV_LOAD_IMAGE_COLOR);
	
	cv::Mat Mt(image);
	cv::Mat outIm; 
	outIm = cv::Mat_<cv::Vec3b>(2048, 2048, (uchar)g_NoHeightValue);
	cv::resize(Mt, outIm, outIm.size());
	/*
	for (int y = 0; y < Mt.rows; ++y)
	{
		const cv::Vec3b *inPtr = Mt.ptr<cv::Vec3b>(y);
		cv::Vec3b *outPtr = outIm.ptr<cv::Vec3b>(y);
		for (int x = 0; x < Mt.cols; ++x)
		{
			outPtr[x].val[0] = inPtr[x].val[0];
			outPtr[x].val[1] = inPtr[x].val[1];
			outPtr[x].val[2] = inPtr[x].val[2];
		}
	}*/
	cv::imwrite(output_filename, outIm);
}

const std::string currentDateTime()
{
	time_t     now = time(0);
	struct tm  tstruct;
	char       buf[80];
	tstruct = *localtime(&now);
	// Visit http://en.cppreference.com/w/cpp/chrono/c/strftime
	// for more information about date/time format
	strftime(buf, sizeof(buf), "%Y_%m_%d_%H_%M_%S", &tstruct);

	return buf;
}

void fisheyeTest::InitLog()
{
	m_count_process = 0;
	m_start_time = currentDateTime();
}
void fisheyeTest::BuildLogFile(const char * output_folder,int type)
{
	std::string tm = currentDateTime();
	std::string filename(output_folder);
	filename.append("OpenCV_log_");
	filename.append(tm.c_str());
	filename.append(".txt");
	ofstream outputFile;
	outputFile.open(filename.c_str());
	std::string data;
	data = "Function: ";
	data.append(m_func_name.c_str());
	outputFile << data.c_str() << std::endl;
	data = "Path: ";
	outputFile << data.c_str() << output_folder << std::endl;
	data = "Number of Images: ";
	outputFile << data.c_str() << m_count_process << std::endl;
	data = "Start: ";
	outputFile << data.c_str() << m_start_time.c_str() << std::endl;
	data = "End: ";
	outputFile << data.c_str() << tm.c_str() << std::endl;
	if (type == 1)
	{
		data = "[Param]";
		outputFile << data.c_str() << std::endl;
		outputFile << m_cam_config.focallen << std::endl;
		outputFile << m_cam_config.r1 << std::endl;
		outputFile << m_cam_config.r2 << std::endl;
		outputFile << m_cam_config.r3 << std::endl;
		outputFile << m_cam_config.ppx << std::endl;
		outputFile << m_cam_config.ppy << std::endl;
		outputFile << m_dest_focal_length << std::endl;
	}
	if (type == 2)
	{
		data = "[Param]";
		outputFile << data.c_str() << std::endl;
		outputFile << m_dest_resolution << std::endl;
	}
	outputFile.close();
	return;
}

void fisheyeTest::MergeVirb360Image(const char * filename, const char * output_filename)
{

	IplImage * image = cvLoadImage(filename, CV_LOAD_IMAGE_COLOR);
	int destRow = 1536;
	int destCol = 4096;
	cv::Mat Mt(image);
	cv::Mat resizeIm;
	resizeIm = cv::Mat_<cv::Vec3b>(2048, 4096, (uchar)g_NoHeightValue);
	cv::resize(Mt, resizeIm, resizeIm.size());
	
	cv::Mat outIm;
	outIm = cv::Mat_<cv::Vec3b>(destRow, destCol, (uchar)g_NoHeightValue);

	for (int y = 0; y < destRow; ++y)
	{
		const cv::Vec3b *inPtr = resizeIm.ptr<cv::Vec3b>(y);
		cv::Vec3b *outPtr = outIm.ptr<cv::Vec3b>(y);
		for (int x = 0; x < resizeIm.cols; ++x)
		{
			outPtr[x].val[0] = inPtr[x].val[0];
			outPtr[x].val[1] = inPtr[x].val[1];
			outPtr[x].val[2] = inPtr[x].val[2];
		}
	}

	cv::imwrite(output_filename, outIm);

	m_dest_resolution = destRow;
	m_func_name = "MergeVirb360Image";
	m_count_process++;
	resizeIm.release();
	outIm.release();
	Mt.release();
	cvReleaseImage(&image);
}

void fisheyeTest::ChangeResolution(const char * filename, const char * output_filename, int resolutionX, int resolutionY)
{

	IplImage * image = cvLoadImage(filename, CV_LOAD_IMAGE_COLOR);
	int row = resolutionX;
	int col = resolutionY;
	cv::Mat Mt(image);
	cv::Mat outIm;
	outIm = cv::Mat_<cv::Vec3b>(row, col, (uchar)g_NoHeightValue);
	cv::resize(Mt, outIm, outIm.size());

	cv::imwrite(output_filename, outIm);

	m_dest_resolution = row;
	m_func_name = "ChangeResolution";
	m_count_process++;
	outIm.release();
	Mt.release();
	cvReleaseImage(&image);
}

void fisheyeTest::ChangeResolution(const char * filename, const char * output_filename, int resolution)
{

	IplImage * image = cvLoadImage(filename, CV_LOAD_IMAGE_COLOR);
	int row = resolution;
	int col = resolution;
	cv::Mat Mt(image);
	cv::Mat outIm;
	outIm = cv::Mat_<cv::Vec3b>(row, col, (uchar)g_NoHeightValue);
	cv::resize(Mt, outIm, outIm.size());


	cv::imwrite(output_filename, outIm);

	m_dest_resolution = row;
	m_func_name = "ChangeResolution";
	m_count_process++;
	outIm.release();
	Mt.release();
	cvReleaseImage(&image);
}

//void fisheyeTest::undistortFisheyeImage(int idx, const char * filename, const char * output_filename)
//{
//	cv::Matx33d theK = this->K;
//	cv::Vec<double, 5> vecD;
//	vecD(0) = m_cam_config.r1;
//	vecD(1) = m_cam_config.r2;
//	vecD(2) = 0.0;
//	vecD(3) = 0.0;
//	vecD(4) = m_cam_config.r3;
//	cv::Mat theD = cv::Mat(vecD);
//	cv::Matx33d newK = theK;
//
//	cv::Matx33d mt33_eye = cv::Matx33d::eye();
//	cv::Matx33d mt33_eye_inv = mt33_eye.inv(0);
//	IplImage * image = cvLoadImage(filename, CV_LOAD_IMAGE_COLOR);
//	cv::Mat distorted(image);
//	float aspect_ratio = (float)distorted.cols / distorted.rows;
//	cv::Mat undistorted;
//	//float focalLengthInPixels3 = GetFocalLengthInPixel(m_src_focallen[idx], distorted.cols, 1.5f);
//	//float focalLengthInPixels = GetFocalLengthInPixel(m_src_focallen[idx], distorted.cols,1.f);// (distorted.cols* 0.5f) / tan(fov * 0.5f * PI / 180.f);
//	//float focalLengthInPixels2 = GetFocalLengthInPixelFrom36mmFocalLength(m_src_focallen[idx], distorted.cols);// (distorted.cols* 0.5f) / tan(fov * 0.5f * PI / 180.f);
//	//float focalLengthInPixels4 = GetFocalLengthInPixelFrom36mmFocalLength(22.f, distorted.cols);// (distorted.cols* 0.5f) / tan(fov * 0.5f * PI / 180.f);
//
//	float focalLengthInPixels = (m_cam_config.focallen / 35.f) * distorted.cols;
//	//double balance = 0.5f;
//	theK(0, 0) = focalLengthInPixels;
//	theK(1, 1) = focalLengthInPixels;// / aspect_ratio;
//	theK(0, 2) = distorted.cols * 0.5f + distorted.cols * m_cam_config.ppx;
//	theK(1, 2) = distorted.rows * 0.5f + distorted.rows * m_cam_config.ppy;
//
//	cv::Size size = distorted.size();
//	//float new_focalLen3 = GetFocalLengthInPixel(m_dest_focallen, distorted.cols, 1.5f);
//	//float new_focalLen2 = GetFocalLengthInPixel(m_dest_focallen, distorted.cols,1.f);
//	//float new_focalLen = GetFocalLengthInPixelFrom36mmFocalLength(m_dest_focallen, distorted.cols); //781.0178f;
//	cv::Mat map1, map2;
//	float new_focalLen = (m_dest_focal_length / 35.f) * distorted.cols; //m_dest_focallen;
//	newK(0, 0) = new_focalLen;// 647.4f;// focalLengthInPixels;
//	newK(1, 1) = new_focalLen;// / aspect_ratio;// 647.4f;// focalLengthInPixels;
//	newK(0, 2) = size.width * 0.5f;
//	newK(1, 2) = size.height * 0.5f;
//	//cv::Mat mt;
//	cv::fisheye::initUndistortRectifyMap(theK, theD, cv::Matx33d::eye(), newK, size, CV_32FC1, map1, map2);
//	cv::remap(distorted, undistorted, map1, map2, cv::INTER_LINEAR, cv::BORDER_CONSTANT);
//
//	//cv::undistort(distorted, undistorted, theK, theD, newK);
//	//cv::Mat undistorted(destImage);
//	//cv::flip(undistorted.t(), undistorted, 1 );
//
//	cv::imwrite(output_filename, undistorted);
//
//
//	m_func_name = "Undistort Images";
//	m_count_process++;
//	map1.release();
//	map2.release();
//	distorted.release();
//	undistorted.release();
//	theD.release();
//	cvReleaseImage(&image);
//	return;
//}

void fisheyeTest::undistortImageWithCamConfig(int idx, const char * filename, const char * output_filename)
{
	cv::Matx33d theK = this->K;
	cv::Vec<double, 5> vecD;
	vecD(0) = m_cam_config.r1;
	vecD(1) = m_cam_config.r2;
	vecD(2) = 0.0;
	vecD(3) = 0.0;
	vecD(4) = m_cam_config.r3;
	cv::Mat theD = cv::Mat(vecD);
	cv::Matx33d newK = theK;

	cv::Matx33d mt33_eye = cv::Matx33d::eye();
	cv::Matx33d mt33_eye_inv = mt33_eye.inv(0);
	IplImage * image = cvLoadImage(filename, CV_LOAD_IMAGE_COLOR);
	cv::Mat distorted(image);
	float aspect_ratio = (float)distorted.cols / distorted.rows;
	cv::Mat undistorted;
	//float focalLengthInPixels3 = GetFocalLengthInPixel(m_src_focallen[idx], distorted.cols, 1.5f);
	//float focalLengthInPixels = GetFocalLengthInPixel(m_src_focallen[idx], distorted.cols,1.f);// (distorted.cols* 0.5f) / tan(fov * 0.5f * PI / 180.f);
	//float focalLengthInPixels2 = GetFocalLengthInPixelFrom36mmFocalLength(m_src_focallen[idx], distorted.cols);// (distorted.cols* 0.5f) / tan(fov * 0.5f * PI / 180.f);
	//float focalLengthInPixels4 = GetFocalLengthInPixelFrom36mmFocalLength(22.f, distorted.cols);// (distorted.cols* 0.5f) / tan(fov * 0.5f * PI / 180.f);

	float focalLengthInPixels = (m_cam_config.focallen / 35.f) * distorted.cols;
	//double balance = 0.5f;
	theK(0, 0) = focalLengthInPixels;
	theK(1, 1) = focalLengthInPixels;// / aspect_ratio;
	theK(0, 2) = distorted.cols * 0.5f + distorted.cols * m_cam_config.ppx;
	theK(1, 2) = distorted.rows * 0.5f + distorted.rows * m_cam_config.ppy;

	cv::Size size = distorted.size();
	//float new_focalLen3 = GetFocalLengthInPixel(m_dest_focallen, distorted.cols, 1.5f);
	//float new_focalLen2 = GetFocalLengthInPixel(m_dest_focallen, distorted.cols,1.f);
	//float new_focalLen = GetFocalLengthInPixelFrom36mmFocalLength(m_dest_focallen, distorted.cols); //781.0178f;
	cv::Mat map1, map2;
	float new_focalLen = (m_dest_focal_length / 35.f) * distorted.cols; //m_dest_focallen;
	newK(0, 0) = new_focalLen;// 647.4f;// focalLengthInPixels;
	newK(1, 1) = new_focalLen;// / aspect_ratio;// 647.4f;// focalLengthInPixels;
	newK(0, 2) = size.width * 0.5f;
	newK(1, 2) = size.height * 0.5f;
	//cv::Mat mt;
	cv::initUndistortRectifyMap(theK, theD, cv::Matx33d::eye(), newK, size, CV_32FC1, map1, map2);
	cv::remap(distorted, undistorted, map1, map2, cv::INTER_LINEAR, cv::BORDER_CONSTANT);
	
	//cv::undistort(distorted, undistorted, theK, theD, newK);
	//cv::Mat undistorted(destImage);
	//cv::flip(undistorted.t(), undistorted, 1 );

	cv::imwrite(output_filename, undistorted);


	m_func_name = "Undistort Images";
	m_count_process++;
	map1.release();
	map2.release();
	distorted.release();
	undistorted.release();
	theD.release();
	cvReleaseImage(&image);
	return;
}

void fisheyeTest::undistortImage(int idx,const char * filename, const char * output_filename)
{
	cv::Matx33d theK = this->K;
	cv::Vec<double, 5> vecD;
	vecD(0) = m_r1[idx];
	vecD(1) = m_r2[idx];
	vecD(2) = 0.0;
	vecD(3) = 0.0;
	vecD(4) = m_r3[idx];
	cv::Mat theD = cv::Mat(vecD);
	cv::Matx33d newK = theK;
	
	cv::Matx33d mt33_eye = cv::Matx33d::eye();
	cv::Matx33d mt33_eye_inv = mt33_eye.inv(0);
	IplImage * image = cvLoadImage(filename, CV_LOAD_IMAGE_COLOR);
	cv::Mat distorted(image);
	float aspect_ratio = (float)distorted.cols / distorted.rows;
	cv::Mat undistorted;
	//float focalLengthInPixels3 = GetFocalLengthInPixel(m_src_focallen[idx], distorted.cols, 1.5f);
	//float focalLengthInPixels = GetFocalLengthInPixel(m_src_focallen[idx], distorted.cols,1.f);// (distorted.cols* 0.5f) / tan(fov * 0.5f * PI / 180.f);
	//float focalLengthInPixels2 = GetFocalLengthInPixelFrom36mmFocalLength(m_src_focallen[idx], distorted.cols);// (distorted.cols* 0.5f) / tan(fov * 0.5f * PI / 180.f);
	//float focalLengthInPixels4 = GetFocalLengthInPixelFrom36mmFocalLength(22.f, distorted.cols);// (distorted.cols* 0.5f) / tan(fov * 0.5f * PI / 180.f);

	float focalLengthInPixels = (m_src_focallen[idx] / 35.f) * distorted.cols;
	//double balance = 0.5f;
	theK(0, 0) = focalLengthInPixels;
	theK(1, 1) = focalLengthInPixels;// / aspect_ratio;
	theK(0, 2) = distorted.cols * 0.5f + distorted.cols * m_ppx[idx];
	theK(1, 2) = distorted.rows * 0.5f + distorted.rows * m_ppy[idx];
	
	cv::Size size = distorted.size();
	//float new_focalLen3 = GetFocalLengthInPixel(m_dest_focallen, distorted.cols, 1.5f);
	//float new_focalLen2 = GetFocalLengthInPixel(m_dest_focallen, distorted.cols,1.f);
	//float new_focalLen = GetFocalLengthInPixelFrom36mmFocalLength(m_dest_focallen, distorted.cols); //781.0178f;
	cv::Mat map1, map2;
	float new_focalLen = (m_dest_focallen[idx] / 35.f) * distorted.cols; //m_dest_focallen;
	newK(0, 0) = new_focalLen;// 647.4f;// focalLengthInPixels;
	newK(1, 1) = new_focalLen;// / aspect_ratio;// 647.4f;// focalLengthInPixels;
	newK(0, 2) = size.width * 0.5f;
	newK(1, 2) = size.height * 0.5f;
	cv::Mat mt;
	cv::initUndistortRectifyMap(theK, theD, cv::Matx33d::eye(), newK, size, CV_32FC1, map1, map2);
	cv::remap(distorted, undistorted, map1, map2, cv::INTER_LINEAR, cv::BORDER_CONSTANT);
	
	//cv::undistort(distorted, undistorted, theK, theD, newK);
	//cv::Mat undistorted(destImage);
//	cv::flip(undistorted.t(), undistorted, 1 );

	cv::imwrite(output_filename, undistorted);
	return;
}

bool fisheyeTest::LoadConfig(char * filename)
{
	WCHAR CurrentDirectory[MAX_PATH];
	CHAR CurrentDirectoryC[MAX_PATH];
	GetCurrentDirectoryW(MAX_PATH, CurrentDirectory);
	CHAR targetFilename[MAX_PATH];
	wcstombs(CurrentDirectoryC, CurrentDirectory, MAX_PATH);
	sprintf_s(targetFilename, "%s\\%s", CurrentDirectoryC,filename);
	PRIVATEPROFILE config_file;
	BOOL isRes = config_file.OpenFile(targetFilename);
	if (!isRes)
	{
		return false;
	}
	LPPRIVATEPROFILESECTION section = config_file.GetSection("Config");
	LPPRIVATEPROFILEKEY key = section->GetKey("Image1Directory");
	if (key)
	{
		sprintf_s(m_process_directory1, "%s", key->GetValuesString());
	}
	key = section->GetKey("Image2Directory");
	if (key)
	{
		sprintf_s(m_process_directory2, "%s", key->GetValuesString());
	}
	key = section->GetKey("Image3Directory");
	if (key)
	{
		sprintf_s(m_process_directory3, "%s", key->GetValuesString());
	}
	key = section->GetKey("Image4Directory");
	if (key)
	{
		sprintf_s(m_process_directory4, "%s", key->GetValuesString());
	}
	
	key = section->GetKey("IsDir");
	if (key)
		m_is_dir = key->GetValuesInt();


	key = section->GetKey("DestFocalLen1");
	if (key)
		m_dest_focallen[0] = key->GetValuesFloat();

	key = section->GetKey("DestFocalLen2");
	if (key)
		m_dest_focallen[1] = key->GetValuesFloat();

	key = section->GetKey("DestFocalLen3");
	if (key)
		m_dest_focallen[2] = key->GetValuesFloat();

	key = section->GetKey("DestFocalLen4");
	if (key)
		m_dest_focallen[3] = key->GetValuesFloat();

	key = section->GetKey("Image1FocalLen");
	if (key)
		m_src_focallen[0] = key->GetValuesFloat();
	key = section->GetKey("Image2FocalLen");
	if (key)
		m_src_focallen[1] = key->GetValuesFloat();
	key = section->GetKey("Image3FocalLen");
	if (key)
		m_src_focallen[2] = key->GetValuesFloat();
	key = section->GetKey("Image4FocalLen");
	if (key)
		m_src_focallen[3] = key->GetValuesFloat();

	key = section->GetKey("Image1R1");
	if (key)
		m_r1[0] = key->GetValuesFloat();
	key = section->GetKey("Image2R1");
	if (key)
		m_r1[1] = key->GetValuesFloat();
	key = section->GetKey("Image3R1");
	if (key)
		m_r1[2] = key->GetValuesFloat();
	key = section->GetKey("Image4R1");
	if (key)
		m_r1[3] = key->GetValuesFloat();

	key = section->GetKey("Image1R2");
	if (key)
		m_r2[0] = key->GetValuesFloat();
	key = section->GetKey("Image2R2");
	if (key)
		m_r2[1] = key->GetValuesFloat();
	key = section->GetKey("Image3R2");
	if (key)
		m_r2[2] = key->GetValuesFloat();
	key = section->GetKey("Image4R2");
	if (key)
		m_r2[3] = key->GetValuesFloat();

	key = section->GetKey("Image1R3");
	if (key)
		m_r3[0] = key->GetValuesFloat();
	key = section->GetKey("Image2R3");
	if (key)
		m_r3[1] = key->GetValuesFloat();
	key = section->GetKey("Image3R3");
	if (key)
		m_r3[2] = key->GetValuesFloat();
	key = section->GetKey("Image4R3");
	if (key)
		m_r3[3] = key->GetValuesFloat();


	key = section->GetKey("Image1PPX");
	if (key)
		m_ppx[0] = key->GetValuesFloat();
	key = section->GetKey("Image2PPX");
	if (key)
		m_ppx[1] = key->GetValuesFloat();
	key = section->GetKey("Image3PPX");
	if (key)
		m_ppx[2] = key->GetValuesFloat();
	key = section->GetKey("Image4PPX");
	if (key)
		m_ppx[3] = key->GetValuesFloat();


	key = section->GetKey("Image1PPY");
	if (key)
		m_ppy[0] = key->GetValuesFloat();
	key = section->GetKey("Image2PPY");
	if (key)
		m_ppy[1] = key->GetValuesFloat();
	key = section->GetKey("Image3PPY");
	if (key)
		m_ppy[2] = key->GetValuesFloat();
	key = section->GetKey("Image4PPY");
	if (key)
		m_ppy[3] = key->GetValuesFloat();
	
	return true;
}




//Convert equirectangular panorama to cube map
// Define our six cube faces. 
// 0 - 3 are side faces, clockwise order
// 4 and 5 are top and bottom, respectively
float faceTransform[6][2] =
{
	{ 0, 0 },
	{ PI / 2, 0 },
	{ PI, 0 },
	{ -PI / 2, 0 },
	{ 0, -PI / 2 },
	{ 0, PI / 2 }
};

cv::Mat markPicture(960, 1920, CV_8UC3, cv::Scalar(255, 255, 255));
cv::Mat copied;
void fisheyeTest::copiedInitialize(cv::Mat &in) {
	copied = in.clone();
}
void GetThetaPhiOfCubemap(int faceId, float u, float v, float& theta, float& phi)
{
	float x, y, z;
	static float halfcubeedge = 1.f;
	if (faceId = 0)//front
	{
		x = halfcubeedge;
		y = u;
		z = v;
	}
	if (faceId = 1)//right
	{
		x = -u;
		y = halfcubeedge;
		z = v;
	}
	if (faceId = 2)//left
	{
		x = u;
		y = -halfcubeedge;
		z = v;
	}
	if (faceId = 3)//back
	{
		x = -halfcubeedge;
		y = -u;
		z = v;
	}
	if (faceId = 4)//buttom
	{
		x = -v;
		y = u;
		z = halfcubeedge;
	}
	if (faceId = 5)//top
	{
		x = v;
		y = u;
		z = -halfcubeedge;
	}
	float dv = sqrt(x*x + y*y + z*z);
	float _x = x / dv;
	float _y = y / dv;
	float _z = z / dv;
	theta = atan2(_y, _x);
	phi = asin(_z);
	return;
}
void UVProjectionOfFisheye200(float theta, float phi, float& u, float& v)
{
	float spherePntX = 0.f;
	float spherePntY = 0.f;
	float spherePntZ = 0.f;

	float r = cos(phi);
	spherePntX = r*cos(theta);
	spherePntY = r*sin(theta);
	spherePntZ = sin(phi);

	u = 0.5f + (spherePntX* -0.5f);
	if (theta >= 0)
		u = u * 0.5f + 0.5f;
	else
		u = (1.f - u) * 0.5f;
	v = 0.5f + (spherePntZ * 0.5f);
	return;
}

//void fisheyeTest::MappingCubemapFromFisheye200(cv::Mat &in, cv::Mat &face,cv::Mat &MapX, cv::Mat &MapY, const int width, const int height)
//{
//	face = cv::Mat(width, height, in.type());
//	// Do actual resampling using OpenCV's remap
//	remap(in, face, MapX, MapY, CV_INTER_LINEAR, cv::BORDER_CONSTANT, cv::Scalar(0, 0, 0));
//	return;
//}

void fisheyeTest::createCubeMapMappingFromFisheye200(cv::Mat &in, cv::Mat &MapX, cv::Mat &MapY, int faceId, const int width, const int height)
{
	int inWidth = in.cols;
	int inHeight = in.rows;

	MapX = cv::Mat(height, width, CV_32F);
	MapY = cv::Mat(height, width, CV_32F);

	const float an = sin(PI / 4);//0.7071067812
	for (int y = 0; y < height; y++) 
	{ 
		for (int x = 0; x < width; x++) 
		{
			// Map face pixel coordinates to [-1, 1] on plane
			float nx = (float)y / (float)height - 0.5f;
			float ny = (float)x / (float)width - 0.5f;
			nx *= 2;
			ny *= 2;
			nx *= an;
			ny *= an;

			float theta, phi;
			float z = 1;
			float dv = sqrt(nx*nx + ny*ny + z*z);
			float _x = x / dv;
			float _y = y / dv;
			float _z = z / dv;
			theta = atan2(_y, _x);
			phi = asin(_z);

			//GetThetaPhiOfCubemap(faceId, nx, ny, theta, phi);
			
			
			float u, v;
			UVProjectionOfFisheye200(theta, phi, u, v);

			u = u * (inWidth - 1);
			v = v * (inHeight - 1);

			MapX.at<float>(x, y) = u;
			MapY.at<float>(x, y) = v;
		}
	}
	return;
}
// Map a part of the equirectangular panorama (in) to a cube face
// (face). The ID of the face is given by faceId. The desired
// width and height are given by width and height. 
void fisheyeTest::createCubeMapFace(cv::Mat &in, cv::Mat &MapX, cv::Mat &MapY, int faceId, const int width,const int height)
{
	int inWidth = in.cols;
	int inHeight = in.rows;

	MapX = cv::Mat(height, width, CV_32F);
	MapY = cv::Mat(height, width, CV_32F);

	// Calculate adjacent (ak) and opposite (an) of the
	// triangle that is spanned from the sphere center 
	//to our cube face.
	const float an = sin(PI / 4);//0.7071067812
	const float ak = cos(PI / 4);//0.7071067812	
	
	const float ftu = faceTransform[faceId][0];
	const float ftv = faceTransform[faceId][1];	

	// For each point in the target image, 
	// calculate the corresponding source coordinates. 
	for (int y = 0; y < height; y++) { // swapped width
		for (int x = 0; x < width; x++) {

			// Map face pixel coordinates to [-1, 1] on plane
			float nx = (float)y / (float)height - 0.5f;
			float ny = (float)x / (float)width - 0.5f;


			nx *= 2;
			ny *= 2;

			// Map [-1, 1] plane coords to [-an, an]
			// thats the coordinates in respect to a unit sphere 
			// that contains our box. 
			nx *= an;
			ny *= an;

			float u, v;

			// Project from plane to sphere surface.
			if (ftv == 0) {
				// Center faces
				u = atan2(nx, ak);
				v = atan2(ny * cos(u), ak);
				u += ftu;

			}
			else if (ftv > 0) {
				// Bottom face 
				float d = sqrt(nx * nx + ny * ny);
				v = PI / 2 - atan2(d, ak); // add minus
				u = -atan2(ny, nx); // swap xy
				//cv::Vec3b color = copied.at<cv::Vec3b>(cv::Point(y, x));
				//color[0] = 0;
				//color[1] = 0;
				//color[2] = 255;
				//copied.at<cv::Vec3b>(cv::Point(y, x)) = color;
				
			}
			else {
				// Top face
				float d = sqrt(nx * nx + ny * ny);
				v = -PI / 2 + atan2(d, ak);
				u = -atan2(-nx, ny); // swap xy
				
			}

			// Map from angular coordinates to [-1, 1], respectively.
			u = u / (PI);
			v = v / (PI / 2);

			// Warp around, if our coordinates are out of bounds. 
			while (v < -1) {
				v += 2;
				u += 1;
			}
			while (v > 1) {
				v -= 2;
				u += 1;
			}

			while (u < -1) {
				u += 2;
			}
			while (u > 1) {
				u -= 2;
			}

			// Map from [-1, 1] to in texture space
			u = u / 2.0f + 0.5f;
			v = v / 2.0f + 0.5f;

			u = u * (inWidth - 1);
			v = v * (inHeight - 1);

			// Save the result for this pixel in map
			MapX.at<float>(x, y) = u;
			MapY.at<float>(x, y) = v;

		}		
	}

	/* produce marked picture
	cv::imwrite("D:\\img\\markedimage\\changedpic.jpg", markPicture);
	*/

	//// Recreate output image if it has wrong size or type. 
	//if (face.cols != width || face.rows != height ||face.type() != in.type()) 
	//{
	//	face = cv::Mat(width, height, in.type());
	//}

	// Do actual resampling using OpenCV's remap
	/*remap(in, face, mapx, mapy,	CV_INTER_LINEAR, cv::BORDER_CONSTANT, cv::Scalar(0, 0, 0));
	mapx.release();
	mapy.release();*/
}


void fisheyeTest::fish2sphere(cv::Mat &inFisheye, cv::Mat &MapX, cv::Mat &MapY,float fishFOV)
{
	float srcWidth = inFisheye.cols;
	float width = inFisheye.cols * 2;
	float height = inFisheye.rows;
	float FOV = (fishFOV / 180.f) * 3.141592654; // FOV of the fisheye, eg: 180 degrees

	MapX = cv::Mat(height, width, CV_32F);
	MapY = cv::Mat(height, width, CV_32F);
	for (int y = 0; y < height; y++) 
	{ 
		for (int x = 0; x < width; x++)
		{
			float pfishX, pfishY;
			float theta, phi, r;
			float psphX, psphY, psphZ;

			// Polar angles
			theta = 2.0 * 3.14159265 * (x / width - 0.5); // -pi to pi
			phi = 3.14159265 * (y / height - 0.5);	// -pi/2 to pi/2

															// Vector in 3D space
			psphX = cos(phi) * sin(theta);
			psphY = cos(phi) * cos(theta);
			psphZ = sin(phi);

			// Calculate fisheye angle and radius
			theta = atan2(psphZ, psphX);
			phi = atan2(sqrt(psphX*psphX + psphZ*psphZ), psphY);
			r = srcWidth * phi / FOV;

			// Pixel in fisheye space
			pfishX = 0.5 * srcWidth + r * cos(theta);
			pfishY = 0.5 * srcWidth + r * sin(theta);

			MapX.at<float>(y, x) = pfishX;
			MapY.at<float>(y, x) = pfishY;
		}
	}


	return;
}
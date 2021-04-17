// lib
const autoprefixer = require('autoprefixer');
const path = require('path');
const webpack = require('webpack');

// plugins
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');

module.exports = (env, argv) => {
    const isProd = argv.mode === 'production';

    // paths
    const distPath = path.resolve(__dirname, './wwwroot');
    const imgDistPath = path.resolve(distPath, './img');
    const srcPath = '.';
    const templatePath = path.resolve(__dirname, './Views/Shared');

    return {
        devtool: isProd ? 'source-map' : 'eval-cheap-module-source-map',
        entry: {
            main: './scripts/main.ts'
        },
        output: {
            filename: 'js/[name].[hash].js',
            path: distPath,
            publicPath: '/',
            libraryTarget: 'var',
            library: 'SecretSanta'
        },

        module: {
            rules: [
                {
                    test: /\.scss$/,
                    use: [
                        { loader: MiniCssExtractPlugin.loader },
                        { loader: 'css-loader' },
                        { loader: 'postcss-loader', options: { postcssOptions: { ident: 'postcss', plugins: () => {autoprefixer()} } } },
                        { loader: 'sass-loader', options: { implementation: require('sass'), sassOptions: { fiber: require('fibers') } } }
                    ]
                },
                {
                    test: /\.css$/,
                    use: [
                        { loader: MiniCssExtractPlugin.loader },
                        { loader: 'css-loader' },
                        { loader: 'postcss-loader', options: { postcssOptions: { ident: 'postcss', plugins: () => {autoprefixer()} } } }
                    ]
                },
                {
                    test: /\.vue$/,
                    loader: 'vue-loader',
                    options: {
                        loaders: {
                            // Since sass-loader (weirdly) has SCSS as its default parse mode, we map
                            // the "scss" and "sass" values for the lang attribute to the right configs here.
                            // other preprocessors should work out of the box, no loader config like this necessary.
                            'scss': 'vue-style-loader!css-loader!sass-loader',
                            'sass': 'vue-style-loader!css-loader!sass-loader?indentedSyntax'
                        }
                    }
                },
                {
                    test: /\.tsx?$/,
                    loader: 'ts-loader',
                    exclude: /node_modules/,
                    options: {
                        appendTsSuffixTo: [/\.vue$/]
                    }
                },
                {
                    test: /\.(png|jpg|gif|svg)$/,
                    loader: 'file-loader',
                    options: {
                        name: '[name].[ext]?[hash]'
                    }
                }
            ]
        },
        plugins: [
            new CleanWebpackPlugin(),

            // // copy src images to wwwroot
            // new CopyWebpackPlugin({
            //     patterns: [
            //         // {from: 'favicon', to: distPath},
            //         // {from: 'icons', to: iconDistPath},
            //         { from: 'images', to: imgDistPath, context: srcPath }
            //     ]
            // }),

            new MiniCssExtractPlugin({
                filename: 'css/[name].[hash].css'
            }),

            new HtmlWebpackPlugin({
                filename: path.resolve(templatePath, './_Layout.cshtml'),
                inject: false,
                minify: false,
                template: path.resolve(templatePath, './_Layout_Template.cshtml')
            })
        ],
        resolve: {
            extensions: ['.ts', '.js', '.json'],
            modules: [
                path.resolve(__dirname, './node_modules'),
                srcPath
            ]
        }
    };
};
import { memo, useContext } from "react";
import { ChangeSpan, PriceIndex, PriceMarket, PricingNode, Query } from "../../gql/codegen/graphql";
import { ChartConfiguration } from "c3";
import _ from "lodash";
import { DateTime } from "luxon";
import { formatDateForSpan } from "../../utils/date";
import { formatCurrency } from "../../utils/number";
import { getColors } from "../../utils/color";
import { EnumMetadataContext } from "../../contexts/enum-metadata.context";

// Must use a dynamic import for c3: https://stackoverflow.com/questions/68596778/next-js-window-is-not-defined
import dynamic from "next/dynamic";
const Chart = dynamic(() => import("../shared/chart.component"), { ssr: false });

type NodeChartProps = {
    span: ChangeSpan;
    pricingNodes: Array<PricingNode>;
    selectedNodes?: Array<PricingNode> | null;
    includeAverages?: boolean;
    height?: number;
};

const getChartData = (
    pricingNodes: Array<PricingNode>,
    selectedNodes: Array<PricingNode> | null,
    priceIndexesAll: Array<PriceIndex>,
    priceMarketsAll: Array<PriceMarket>,
    includeAverages: boolean
) => {
    const averageName = "Average";
    if (includeAverages) {
        const averages = _.chain(_.flatMap(pricingNodes, "prices"))
            .groupBy((result) => [result.intervalEndTimeUtc, result.priceIndexId])
            .map((entries, key) => {
                const intervalEndTimeUtc = _.split(key, ",")[0];
                const priceIndexId = _.split(key, ",")[1];
                return {
                    intervalEndTimeUtc,
                    priceIndexId,
                    lmpPrice: _.meanBy(entries, (entry) => entry.lmpPrice),
                };
            })
            .value();

        pricingNodes = [
            ...pricingNodes,
            {
                name: averageName,
                prices: averages,
            } as PricingNode,
        ];
    }

    if (selectedNodes) {
        const selectedNodeIds = _.map(selectedNodes, "id");
        const filteredPricingNodes: Array<PricingNode> = _.filter(pricingNodes, (pn) => {
            return pn.name === averageName || selectedNodeIds.indexOf(pn.id) > -1;
        });
        pricingNodes = [...filteredPricingNodes];
    }

    const prices = pricingNodes.reduce((acc: any[], pn: PricingNode) => {
        for (const price of pn.prices) {
            acc.push({
                ...pn,
                ...price,
                displayName:
                    pricingNodes.length > 1 || includeAverages
                        ? `${pn.name} - ${_.find(priceMarketsAll, { id: _.find(priceIndexesAll, { id: price.priceIndexId })?.priceMarketId })?.name}`
                        : `${_.find(priceMarketsAll, { id: _.find(priceIndexesAll, { id: price.priceIndexId })?.priceMarketId })?.name}`,
            });
        }
        return acc;
    }, []);

    const pricesByIntervalEndTimeUtc = _.groupBy(prices, "intervalEndTimeUtc");
    const chartSeries = _.uniq(prices.map((p) => p.displayName));
    const chartData: Record<string, any[]> = Object.entries(pricesByIntervalEndTimeUtc).reduce(
        (acc: { x: DateTime[] }, [k, v]: [string, Array<PricingNode>]) => {
            acc.x.push(DateTime.fromISO(k));

            const pricesByIndex: Record<string, number> = v.reduce((acc2: any, p: any) => {
                acc2 = {
                    ...acc2,
                    [p.displayName]: p.lmpPrice,
                };

                return acc2;
            }, {});

            for (const series of chartSeries) {
                if (!acc[series]) {
                    acc[series] = [pricesByIndex[series] ?? null];
                } else {
                    acc[series].push(pricesByIndex[series] ?? null);
                }
            }
            return acc;
        },
        { x: [] }
    );

    return Object.entries(chartData).reduce((acc: any, [k, v]: [string, any[]]) => {
        acc.push([k, ...v]);
        return acc;
    }, []);
};

const getChartConfig: any = (span, height) => {
    return {
        size: { height: height },
        zoom: {
            enabled: true,
        },
        axis: {
            x: {
                type: "timeseries",
                tick: {
                    format: (v) => formatDateForSpan(v, span),
                    count: 6,
                },
            },
            y: {
                tick: {
                    format: (v) => formatCurrency(v, { currency: "USD" }),
                },
            },
        },
        color: {
            pattern: getColors(),
        },
        legend: {
            show: true,
        },
        grid: {
            y: {
                show: false,
            },
        },
        point: {
            show: false,
        },
        tooltip: {
            format: {
                title: (v) => formatDateForSpan(v),
            },
        },
        line: {
            connectNull: true,
        },
    };
};

const NodeChart = memo<NodeChartProps>(function NodeChart(props: NodeChartProps) {
    const { span, pricingNodes, selectedNodes = null, includeAverages = false, height = 350 } = props;
    const { priceIndexes, priceMarkets } = useContext(EnumMetadataContext);

    const chartData = getChartData(pricingNodes, selectedNodes, priceIndexes, priceMarkets, includeAverages);

    const chartConfig: ChartConfiguration = {
        data: {
            x: "x",
            columns: chartData,
        },
        ...getChartConfig(span, height),
    };

    return <Chart config={chartConfig} />;
});

export default NodeChart;
